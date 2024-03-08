// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging.Elements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Econolite.Ode.Messaging.Extensions
{
    public static class Defined
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services) => services
            .AddSingleton<IBuildMessagingConfig, BuildMessagingConfig>()
            .AddSingleton<IHeaderFactory, HeaderFactory>();

        #region Add Source Helper Extensions
        public static IServiceCollection AddMessagingJsonSource<TType>(this IServiceCollection services) => services
            .AddMessagingJsonSource<TType>(_ => { });

        public static IServiceCollection AddMessagingProtobufSource<TType>(this IServiceCollection services) => services
            .AddMessagingProtobufSource<TType>(_ => { });

        public static IServiceCollection AddMessagingCombinedSource<TType>(this IServiceCollection services) => services
            .AddMessagingCombinedSource<TType>(_ => { });

        public static IServiceCollection AddMessagingJsonSource<TType>(this IServiceCollection services, Action<SourceOptions<TType>> optionsAction) => services
            .AddMessaging()
            .Configure(optionsAction)
            .AddTransient<IPayloadSpecialist<TType>, JsonPayloadSpecialist<TType>>()
            .AddTransient<IConsumeResultFactory<TType>, ConsumeResultFactory<TType>>()
            .AddTransient<ISource<TType>, Source<TType>>();

        public static IServiceCollection AddMessagingProtobufSource<TType>(this IServiceCollection services, Action<SourceOptions<TType>> optionsAction) => services
            .AddMessaging()
            .Configure(optionsAction)
            .AddTransient<IPayloadSpecialist<TType>, ProtobufPayloadSpecialist<TType>>()
            .AddTransient<IConsumeResultFactory<TType>, ConsumeResultFactory<TType>>()
            .AddTransient<ISource<TType>, Source<TType>>();

        public static IServiceCollection AddMessagingCombinedSource<TType>(this IServiceCollection services, Action<SourceOptions<TType>> optionsAction) => services
            .AddMessaging()
            .Configure(optionsAction)
            .AddTransient<ProtobufPayloadSpecialist<TType>>()
            .AddTransient<JsonPayloadSpecialist<TType>>()
            .AddTransient<IPayloadSpecialist<TType>, CombinedPayloadSpecialist<TType>>()
            .AddTransient<IConsumeResultFactory<TType>, ConsumeResultFactory<TType>>()
            .AddTransient<ISource<TType>, Source<TType>>();
        #endregion

        #region Add Sink Helper Extensions
        public static IServiceCollection AddMessagingJsonSink<TType>(this IServiceCollection services) where TType : notnull => services
            .AddMessagingJsonSink<TType>(_ => { }, _ => { });

        public static IServiceCollection AddMessagingJsonSink<TType>(this IServiceCollection services, Action<SinkOptions<TType>> sinkOptions) where TType : notnull => services
            .AddMessagingJsonSink<TType>(_ => { }, sinkOptions);

        public static IServiceCollection AddMessagingJsonSink<TType>(this IServiceCollection services, Action<MessageFactoryOptions<TType>> factoryOptions) where TType : notnull => services
            .AddMessagingJsonSink<TType>(factoryOptions, _ => { });

        public static IServiceCollection AddMessagingJsonSink<TType>(this IServiceCollection services, Action<MessageFactoryOptions<TType>> factoryOptions, Action<SinkOptions<TType>> sinkOptions) where TType : notnull => services
            .Configure(sinkOptions)
            .Configure<MessageFactoryOptions<TType>>(_ =>
            {
                // We are going to include the BaseJsonPayload
                _.FuncBuildPayloadElement = _ => new BaseJsonPayload<TType>(_);
                factoryOptions(_);
            })
            .AddTransient<IMessageFactory<TType>, MessageFactory<TType>>()
            .AddTransient<ISink<TType>, Sink<TType>>();

        public static IServiceCollection AddMessagingProtobufSink<TType>(this IServiceCollection services) where TType : notnull => services
            .AddMessagingJsonSink<TType>(_ => { }, _ => { });

        public static IServiceCollection AddMessagingProtobufSink<TType>(this IServiceCollection services, Action<SinkOptions<TType>> sinkOptions) where TType : notnull => services
            .AddMessagingProtobufSink<TType>(_ => { }, sinkOptions);

        public static IServiceCollection AddMessagingProtobufSink<TType>(this IServiceCollection services, Action<MessageFactoryOptions<TType>> factoryOptions) where TType : notnull => services
            .AddMessagingProtobufSink<TType>(factoryOptions, _ => { });

        public static IServiceCollection AddMessagingProtobufSink<TType>(this IServiceCollection services, Action<MessageFactoryOptions<TType>> factoryOptions, Action<SinkOptions<TType>> sinkOptions) where TType : notnull => services
            .Configure(sinkOptions)
            .Configure<MessageFactoryOptions<TType>>(_ =>
            {
                // We are going to include the BaseJsonPayload
                _.FuncBuildPayloadElement = _ => new BaseProtobufPayload<TType>(_);
                factoryOptions(_);
            })
            .AddTransient<IMessageFactory<TType>, MessageFactory<TType>>()
            .AddTransient<ISink<TType>, Sink<TType>>();
        #endregion

        #region Add Generic Source Helper Extensions

        public static IServiceCollection AddMessagingJsonGenericSource<TKey, TType>(this IServiceCollection services, Func<byte[], TKey> funcBuildKey, Action<SourceOptions<TKey, TType>> optionsAction) => services
            .AddMessaging()
            .Configure(optionsAction)
            .AddTransient<IConsumeResultFactory<TKey, TType>, ConsumeResultFactory<TKey, TType>>(_ => new ConsumeResultFactory<TKey, TType>(_ => funcBuildKey(_), new JsonPayloadSpecialist<TType>()))
            .AddTransient<ISource<TKey, TType>, GenericSource<TKey, TType>>();

        #endregion
        
        #region Add Generic Sink Helper Extensions
        
        public static IServiceCollection AddMessagingJsonGenericSink<TKey, TType>(this IServiceCollection services, Action<SinkOptions<TKey, TType>> sinkOptions) where TType : notnull where TKey : notnull => services
            .AddMessagingJsonGenericSink(_ => { }, sinkOptions);
    
        public static IServiceCollection AddMessagingJsonGenericSink<TKey, TType>(this IServiceCollection services, Action<MessageFactoryOptions<TKey, TType>> factoryOptions, Action<SinkOptions<TKey, TType>> sinkOptions) where TType : notnull where TKey : notnull => services
            .Configure(sinkOptions)
            .Configure<MessageFactoryOptions<TKey, TType>>(_ =>
            {
                // We are going to include the BaseJsonPayload
                _.FuncBuildPayloadElement = _ => new BaseJsonPayload<TType>(_);
                factoryOptions(_);
            })
            .AddTransient<IMessageFactory<TKey, TType>, MessageFactory<TKey, TType>>()
            .AddTransient<ISink<TKey, TType>, GenericSink<TKey, TType>>();

        #endregion
    }
}
