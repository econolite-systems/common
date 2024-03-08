// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Monitoring.Metrics.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Econolite.Ode.Monitoring.Metrics
{
    public class MetricsMonitor : BackgroundService
    {
        private readonly MeterListener _meterListener;
        private readonly MetricsMonitorOptions _options;
        private readonly string _computer;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Instrument, long> _observable = new();
        private readonly IMetricSink _metricSink;
        private readonly Guid _hashKey;
        private readonly ILocalMonitor _localMonitor;
        public MetricsMonitor(IMetricSink metricSink, IOptions<MetricsFactoryOptions> factoryOptions, IOptions<MetricsMonitorOptions> monitorOptions, ILocalMonitor localMonitor, ILoggerFactory loggerFactory)
        {
            _localMonitor = localMonitor;
            _metricSink = metricSink;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _meterListener = new MeterListener();
            _options = monitorOptions.Value ?? new();
            _computer = Environment.MachineName;
            _hashKey = Guid.NewGuid();
            _meterListener.InstrumentPublished = (instrument, meterListener) =>
            {
                if (meterListener == _meterListener && instrument.IsObservable && instrument.Meter.Name == factoryOptions.Value.MeterName)
                {
                    meterListener.EnableMeasurementEvents(instrument);
                }
            };
            _meterListener.MeasurementsCompleted = (instrument, _) =>
            {
                _observable.Remove(instrument, out long value);
            };
            _meterListener.SetMeasurementEventCallback<int>(OnMeasurementCallback);
            _meterListener.SetMeasurementEventCallback<long>(OnMeasurementCallback);
            _meterListener.Start();
        }

        void OnMeasurementCallback(Instrument instrument, int measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            if (instrument is ObservableCounter<int>)
            {
                OnCounterMeasurementCallback(instrument, measurement, tags, state);
            }
            else
            {
                OnInstrumentMeasurementCallback(instrument, measurement, tags, state);
            }
        }

        void OnMeasurementCallback(Instrument instrument, long measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            if (instrument is ObservableCounter<long>)
            {
                OnCounterMeasurementCallback(instrument, measurement, tags, state);
            }
            else
            {
                OnInstrumentMeasurementCallback(instrument, measurement, tags, state);
            }
        }

        void OnCounterMeasurementCallback(Instrument instrument, long measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            _observable.AddOrUpdate(instrument, (_) => measurement, (_, old) => measurement - old);
        }

        void OnInstrumentMeasurementCallback(Instrument instrument, long measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            _observable.AddOrUpdate(instrument, (_) => measurement, (_, _) => measurement);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_options.MeasurementPeriod, stoppingToken);
                        _meterListener.RecordObservableInstruments();
                        var monitored = _observable.ToArray();
                        await Task.WhenAll(PublishAsync(monitored, stoppingToken), _localMonitor.OnMonitoringAsync(monitored, stoppingToken));
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex )
                    {
                        _logger.LogError(ex, "Could not complete monitoring loop");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Stopping");
            }
        }
        private async Task PublishAsync(KeyValuePair<Instrument, long>[] array, CancellationToken stoppingToken)
        {
            var message = new MetricMessage()
            {
                Computer = _computer,
                InstanceHash = _hashKey.ToString(),
                Logged = DateTime.UtcNow,
                Source = _options.Source,
                TenantId = new Uuid
                {
                    Value = _options.TenantId.ToString(),
                },
            };
            message.Metrics.AddRange(array.Select(_ => new Metric
            {
                Name = _.Key.Name,
                Value = _.Value,
                Units = _.Key.Unit
            }));
            await _metricSink.SinkAsync(_hashKey, message, stoppingToken);
        }
    }
}
