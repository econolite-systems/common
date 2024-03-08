// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Econolite.Ode.Messaging
{
    public class BuildMessagingConfig : IBuildMessagingConfig
    {
        private readonly string _caLocation;
        private readonly string _clientLocation;
        private readonly KafkaConfigOptions _kafkaConfigOptions;
        private readonly ILogger _logger;

        public BuildMessagingConfig(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _kafkaConfigOptions = new();
             configuration.GetSection("Kafka").Bind(_kafkaConfigOptions);
            (_caLocation, _clientLocation) = EnsureCertsExistAndAreCurrent(CertFilenames.Default);
        }

        protected BuildMessagingConfig(KafkaConfigOptions kafkaConfigOptions, CertFilenames certFilenames, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _kafkaConfigOptions = kafkaConfigOptions;
            (_caLocation, _clientLocation) = EnsureCertsExistAndAreCurrent(certFilenames);
        }

        (string CaLocation, string ClientLocation) EnsureCertsExistAndAreCurrent(CertFilenames certFilenames)
        {
            string calocationresult = string.Empty;
            string clientlocationresult = string.Empty;
            if (!string.IsNullOrWhiteSpace(_kafkaConfigOptions.ssl.ca) || !string.IsNullOrWhiteSpace(_kafkaConfigOptions.ssl.certificate))
            {
                _logger.LogDebug("Using self-signed cert");
                string[] storagelocations = new string[]
                {
                ".",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Econolite", Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location)),
                };

                calocationresult = !string.IsNullOrWhiteSpace(_kafkaConfigOptions.ssl.ca) ? StoreFile(certFilenames.CAFilename, Base64Decode(_kafkaConfigOptions.ssl.ca ?? string.Empty), storagelocations) : string.Empty;
                clientlocationresult = !string.IsNullOrWhiteSpace(_kafkaConfigOptions.ssl.certificate) ? StoreFile(certFilenames.CertFilename, Base64Decode(_kafkaConfigOptions.ssl.certificate ?? string.Empty), storagelocations) : string.Empty;
            }
            else
            {
                _logger.LogDebug("Not using self-signed certs");
            }
            return (calocationresult, clientlocationresult);
        }

        private string StoreFile(string filename, string data, string[] storagelocations)
        {
            var result = string.Empty;
            foreach (var location in storagelocations)
            {
                if (EnsureWorkingDirectoryExists(location))
                {
                    var filenametotry = Path.Combine(location, filename);
                    if (VerifyFileData(filenametotry, data))
                    {
                        result = filenametotry;
                        break;
                    }
                }
            }
            return result;
        }

        private bool VerifyFileData(string filename, string data)
        {
            bool result = false;
            try
            {
                if (File.Exists(filename))
                {
                    if (File.ReadAllText(filename) == data)
                    {
                        result = true;
                    }
                    else
                    {
                        File.WriteAllText(filename, data);
                        result = true;
                    }
                }
                else
                {
                    File.WriteAllText(filename, data);
                    result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        private bool EnsureWorkingDirectoryExists(string directory)
        {
            var result = false;
            if (Directory.Exists(directory))
            {
                result = true;
            }
            else
            {
                try
                {
                    if (EnsureWorkingDirectoryExists((Directory.GetParent(directory) ?? throw new NullReferenceException()).FullName))
                    {
                        Directory.CreateDirectory(directory);
                        result = true;
                    }

                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        public ConsumerConfig BuildConsumerClientConfig(ConsumerOptions options)
        {
            var result = new ConsumerConfig
            {
                GroupId = SelectConsumerGroup(options),

                // These are the settings that work without problems. Note that the commit is auto but the saving of the offset
                // is not. We will save the offset as part of the Complete
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false,
                AutoOffsetReset = AutoOffsetReset.Latest,

                BootstrapServers = _kafkaConfigOptions.bootstrap.servers
            };

            AddSecutityInfo(result);
            AddOverrides(options, result);

            return result;
        }

        public ProducerConfig BuildProducerClientConfig(ProducerOptions options)
        {
            var result = new ProducerConfig
            {
                Acks = Acks.Leader,
                BootstrapServers = _kafkaConfigOptions.bootstrap.servers
            };

            AddSecutityInfo(result);
            AddOverrides(options, result);

            return result;
        }

        private string SelectConsumerGroup(ConsumerOptions consumerOptions)
        {
            return SelectFromOptions((consumerOptions?.ConsumerGroupOverride, consumerOptions?.ConsumerGroupSuffix));
        }

        private string SelectFromOptions((string? Override, string? Suffix) options)
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(options.Override))
            {
                result.Append(options.Override);
            }
            else
            {
                result.Append(_kafkaConfigOptions.GroupId);
                if (!string.IsNullOrEmpty(options.Suffix))
                {
                    result.Append($"-{options.Suffix}");
                }
            }
            return result.ToString();
        }

        private void AddSecutityInfo(ClientConfig result)
        {
            // Sets the Security Protocol
            if (_kafkaConfigOptions.security.protocol == "SASL_SSL")
            {
                result.SecurityProtocol = SecurityProtocol.SaslSsl;
                if (! string.IsNullOrEmpty(_caLocation))
                    result.SslCaLocation = _caLocation;
                if (!string.IsNullOrEmpty(_clientLocation))
                    result.SslCertificateLocation = _clientLocation;
            }
            else if (_kafkaConfigOptions.security.protocol == "SASL_PLAIN")
            {
                result.SecurityProtocol = SecurityProtocol.SaslPlaintext;
            }
            else
            {
                result.SecurityProtocol = SecurityProtocol.Plaintext;
            }
            _logger.LogDebug("SecurityProtocol: {@SecurityProtocol}", result.SecurityProtocol);

            // Sets the Sasl Mechanism
            if (_kafkaConfigOptions.sasl.mechanism == "SCRAM-SHA-512")
            {
                result.SaslMechanism = SaslMechanism.ScramSha512;
                result.SaslUsername = _kafkaConfigOptions.sasl.username;
                result.SaslPassword = _kafkaConfigOptions.sasl.password;
            }
            _logger.LogDebug("SASL Mechanism: {@SaslMechanism}", result.SaslMechanism ?? SaslMechanism.Plain);
            _logger.LogDebug("SaslUsername {@SaslUsername}", result.SaslUsername ?? "");
        }

        private static void AddOverrides(MessagingOptions options, ClientConfig result)
        {
            if (options?.IncludeInternalDebug ?? false)
            {
                result.Set("debug", "all");
            }

            if (options?.ConfigOverrides?.Any() ?? false)
            {
                foreach (var item in options.ConfigOverrides)
                {
                    if (item.Key != "debug")
                    {
                        result.Set(item.Key, item.Value);
                    }
                }
            }
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class BuildMessagingConfig<TKey, TValue> : BuildMessagingConfig, IBuildMessagingConfig<TKey, TValue>
    {
        public BuildMessagingConfig(IOptions<KafkaConfigOptions<TKey, TValue>> options, ILoggerFactory loggerFactory) : base(options.Value, options.Value.CertFilenames, loggerFactory)
        {

        }
    }
}
