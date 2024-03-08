// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Cronos;

namespace Econolite.Ode.Common.Scheduler.Base.Timers.Impl
{
    public class GeneralTimer : IPeriodicTimer
    {
        private Task? _timerTask;
        private Func<Task>? _task;
        private readonly CronosPeriodicTimer _timer;
        private readonly CancellationTokenSource _cts = new();

        public GeneralTimer(string expresssion)
        {
            _timer = new CronosPeriodicTimer(expresssion, CronFormat.IncludeSeconds);
        }

        public void Start(Func<Task> task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _timerTask = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    if (_task != null)
                        await _timer.WaitForNextTickAsync(_cts.Token).ContinueWith(_ => _task());
                }
            }
            catch (OperationCanceledException)
            {
                // Stopping, this is to be expected
            }
        }

        public async Task StopAsync()
        {
            if (_timerTask is null)
            {
                return;
            }

            _cts.Cancel();
            await _timerTask;
            _cts.Dispose();
            Console.WriteLine("Canceled");
        }
    }
}
