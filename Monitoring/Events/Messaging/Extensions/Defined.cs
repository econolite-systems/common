// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Econolite.Ode.Monitoring.Events.Messaging.Extensions
{
    public static class Defined
    {
        public static IServiceCollection AddEventSourceServices(this IServiceCollection services, IConfiguration configuration, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddEventSourceServices(configuration[configPath] ?? Consts.DEFAULT_CHANNEL);
        public static IServiceCollection AddEventSourceServices(this IServiceCollection services, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddEventSourceServices(_ =>
            {
                _.DefaultChannel = defaultChannel;
            });
        public static IServiceCollection AddEventSourceServices(this IServiceCollection services, Action<SourceOptions<UserEventMessage>> configure) => services
            .Configure(configure)
            .AddMessaging()
            .AddTransient<IPayloadSpecialist<UserEventMessage>, ProtobufPayloadSpecialist<UserEventMessage>>()
            .AddTransient<IConsumeResultFactory<UserEventMessage>, ConsumeResultFactory<UserEventMessage>>()
            .AddTransient<ISource<UserEventMessage>, Source<UserEventMessage>>()
            .AddEventSinkServices(Guid.Empty)
            .AddTransient<IEventSource, EventSource>();

        public static IServiceCollection AddEventSinkServices(this IServiceCollection services, IConfiguration configuration, Guid defaultTenantId, string configPath = Consts.DEFAULT_CHANNEL_CONFIG_PATH) => services
            .AddEventSinkServices(defaultTenantId, configuration[configPath] ?? Consts.DEFAULT_CHANNEL);
        public static IServiceCollection AddEventSinkServices(this IServiceCollection services, Guid defaultTenantId, string defaultChannel = Consts.DEFAULT_CHANNEL) => services
            .AddEventSinkServices(defaultTenantId, _ =>
            {
                _.DefaultChannel = defaultChannel;
            });
        public static IServiceCollection AddEventSinkServices(this IServiceCollection services, Guid defaultTenantId, Action<SinkOptions<UserEventMessage>> action) => services
            .Configure<UserEventSinkOptions>(_ =>
            {
                _.DefaultTenantId = defaultTenantId;
            })
            .Configure<MessageFactoryOptions<UserEventMessage>>(_ =>
            {
                _.TenantId = defaultTenantId;
                _.FuncBuildPayloadElement = _ => new BaseProtobufPayload<UserEventMessage>(_);
            })
            .Configure(action)
            .AddMessaging()
            .AddTransient<IMessageFactory<UserEventMessage>, MessageFactory<UserEventMessage>>()
            .AddTransient<ISink<UserEventMessage>, Sink<UserEventMessage>>()
            .AddSingleton<ISinkContext, SinkContext>()
            .AddSingleton<UserEventFactory>()
            .AddTransient<IEventSink, EventSink>();


        public static UserEventMessage ToProtobuf(this UserEvent userEvent) => new()
        {
            Category = userEvent.Category.ToProtobuf(),
            Computer = userEvent.Computer,
            Details  = userEvent.Details,
            Level = userEvent.Level.ToProtobuf(),
            Logged = userEvent.Logged,
            LogName = userEvent.LogName.ToProtobuf(),
            Source = userEvent.Source,
            TenantId = userEvent.TenantId.ToProtobuf()
        };

        public static Category ToProtobuf(this Events.Category category) => category switch
        {
            Events.Category.Operational => Category.Operational,
            Events.Category.User => Category.User,
            Events.Category.Server => Category.Server,
            _ => Category.CategoryUnknown,
        };

        public static Events.Category FromProtobuf(this Category category) => category switch
        {
            Category.Operational => Events.Category.Operational,
            Category.User => Events.Category.User,
            Category.Server => Events.Category.Server,
            _ => Events.Category.CategoryUnknown,
        };

        public static EventLevel ToProtobuf(this Events.EventLevel level) => level switch
        {
            Events.EventLevel.Critical => EventLevel.Critical,
            Events.EventLevel.Error => EventLevel.Error,
            Events.EventLevel.Warning => EventLevel.Warning,
            Events.EventLevel.Information => EventLevel.Information,
            Events.EventLevel.Debug => EventLevel.Debug,
            _ => EventLevel.EventLevelUnknown,
        };

        public static Events.EventLevel FromProtobuf(this EventLevel level) => level switch
        {
            EventLevel.Critical => Events.EventLevel.Critical,
            EventLevel.Error => Events.EventLevel.Error,
            EventLevel.Warning => Events.EventLevel.Warning,
            EventLevel.Information => Events.EventLevel.Information,
            EventLevel.Debug => Events.EventLevel.Debug,
            _ => Events.EventLevel.EventLevelUnknown,
        };

        public static LogName ToProtobuf(this Events.LogName logName) => logName switch
        {
            Events.LogName.SystemEvent => LogName.SystemEvent,
            Events.LogName.Audit => LogName.Audit,
            _ => LogName.LogNameUnknown,
        };

        public static Events.LogName FromProtobuf(this LogName logName) => logName switch
        {
            LogName.SystemEvent => Events.LogName.SystemEvent,
            LogName.Audit => Events.LogName.Audit,
            _ => Events.LogName.LogNameUnknown,
        };

        public static Uuid ToProtobuf(this Guid guid) => new Uuid
        {
            Value = guid.ToString(),
        };

        public static Guid FromProtobuf(this Uuid uuid) => Guid.Parse(uuid.Value);

        internal static UserEvent FromProtobuf(this UserEventMessage userEvent, ISinkContext sinkContext) => new(sinkContext)
        {
            Category = userEvent.Category.FromProtobuf(),
            Computer = userEvent.Computer,
            Details = userEvent.Details,
            Level = userEvent.Level.FromProtobuf(),
            Logged = userEvent.Logged,
            LogName = userEvent.LogName.FromProtobuf(),
            Source = userEvent.Source,
            TenantId = userEvent.TenantId.FromProtobuf(),
        };
    }
}
