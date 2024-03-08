// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Options;
using System;

namespace Econolite.Ode.Monitoring.Events
{
    public class UserEventFactory
    {
        private readonly UserEventFactoryOptions _options;
        private readonly string _computer;
        private readonly ISinkContext _sinkContext;
        public UserEventFactory(IOptions<UserEventFactoryOptions> options, ISinkContext sinkContext)
        {
            _sinkContext = sinkContext;
            _options = options.Value ?? new UserEventFactoryOptions();
            _computer = string.IsNullOrEmpty(_options.Computer) ? Environment.MachineName : _options.Computer;
        }

        /// <summary>
        /// Creates a UserEvent using only the default computer from the DI configured UserEventFactoryOptions
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="eventLevel"></param>
        /// <param name="source"></param>
        /// <param name="tenantId"></param>
        /// <param name="category"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(LogName logName, EventLevel eventLevel, string source, Guid tenantId, Category category, string details) => new(_sinkContext)
        {
            Category = category,
            Source = source,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = _computer,
            LogName = logName,
            TenantId = tenantId,
        };

        /// <summary>
        /// Creates a UserEvent using only the default for the tenantId and the computer from the DI configured UserEventFactoryOptions
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="eventLevel"></param>
        /// <param name="source"></param>
        /// <param name="category"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(LogName logName, EventLevel eventLevel, string source, Category category, string details) => new(_sinkContext)
        {
            Category = category,
            Source = string.IsNullOrEmpty(source) ? _options.DefaultSource : source,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = _computer,
            LogName = logName,
            TenantId = _options.DefaultTenantId,
        };

        /// <summary>
        /// Creates a UserEvent using only the default for the tenantId, source and the computer from the DI configured UserEventFactoryOptions
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="eventLevel"></param>
        /// <param name="source"></param>
        /// <param name="category"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(LogName logName, EventLevel eventLevel, Category category, string details) => new(_sinkContext)
        {
            Category = category,
            Source = _options.DefaultSource,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = _computer,
            LogName = logName,
            TenantId = _options.DefaultTenantId,
        };

        /// <summary>
        /// Creates a UserEvent using the defaults in the DI configured UserEventFactoryOptions except the source
        /// </summary>
        /// <param name="eventLevel"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(EventLevel eventLevel, string source, string details) => new(_sinkContext)
        {
            Category = _options.DefaultCategory,
            Source = source,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = _computer,
            LogName = _options.DefaultLogName,
            TenantId = _options.DefaultTenantId,
        };

        /// <summary>
        /// Creates a UserEvent using the defaults in the DI configured UserEventFactoryOptions
        /// </summary>
        /// <param name="eventLevel"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(EventLevel eventLevel, string details) => new(_sinkContext)
        {
            Category = _options.DefaultCategory,
            Source = _options.DefaultSource,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = _computer,
            LogName = _options.DefaultLogName,
            TenantId = _options.DefaultTenantId,
        };

        /// <summary>
        /// Creates a UserEvent using the defaults in the supplied UserEventFactoryOptions will use the computer name from the DI if none is included in the supplied options
        /// </summary>
        /// <param name="eventFactoryOptions"></param>
        /// <param name="eventLevel"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public UserEvent BuildUserEvent(UserEventFactoryOptions eventFactoryOptions, EventLevel eventLevel, string details) => new(_sinkContext)
        {
            Category = eventFactoryOptions.DefaultCategory,
            Source = eventFactoryOptions.DefaultSource,
            Level = eventLevel,
            Logged = DateTime.UtcNow,
            Details = details,
            Computer = string.IsNullOrEmpty(eventFactoryOptions.Computer) ? _computer : eventFactoryOptions.Computer,
            LogName = eventFactoryOptions.DefaultLogName,
            TenantId = eventFactoryOptions.DefaultTenantId,
        };
    }
}
