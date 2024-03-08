// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Messaging.Extensions
{
    public static class ConfigBuilderExtensions
    {
        public static ProducerBuilder<TKey, TValue> AddLogging<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder, ILogger logger)
        {
            builder.SetLogHandler((_, message) =>
            {
                switch (message.Level)
                {
                    case SyslogLevel.Emergency:
                    case SyslogLevel.Alert:
                    case SyslogLevel.Critical:
                        logger.LogCritical(message.Message);
                        break;
                    case SyslogLevel.Error:
                        logger.LogError(message.Message);
                        break;
                    case SyslogLevel.Warning:
                        logger.LogWarning(message.Message);
                        break;
                    case SyslogLevel.Notice:
                    case SyslogLevel.Info:
                    default:
                        logger.LogInformation(message.Message);
                        break;
                    case SyslogLevel.Debug:
                        logger.LogDebug(message.Message);
                        break;
                }
            })
            .SetErrorHandler((_, error) =>
            {
                if (error.IsFatal)
                    logger.LogCritical(error.Reason);
                else
                    logger.LogError(error.Reason);
            });
            return builder;
        }

        public static ConsumerBuilder<TKey, TValue> AddLogging<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder, ILogger logger)
        {
            builder.SetLogHandler((_, message) =>
            {
                switch (message.Level)
                {
                    case SyslogLevel.Emergency:
                    case SyslogLevel.Alert:
                    case SyslogLevel.Critical:
                        logger.LogCritical(message.Message);
                        break;
                    case SyslogLevel.Error:
                        logger.LogError(message.Message);
                        break;
                    case SyslogLevel.Warning:
                        logger.LogWarning(message.Message);
                        break;
                    case SyslogLevel.Notice:
                    case SyslogLevel.Info:
                    default:
                        logger.LogInformation(message.Message);
                        break;
                    case SyslogLevel.Debug:
                        logger.LogDebug(message.Message);
                        break;
                }
            })
            .SetErrorHandler((_, error) =>
            {
                if (error.IsFatal)
                    logger.LogCritical(error.Reason);
                else
                    logger.LogError(error.Reason);
            });
            return builder;
        }

    }
}
