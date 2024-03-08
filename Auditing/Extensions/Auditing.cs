// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Audit.Core;
using Audit.MongoDB.ConfigurationApi;
using Audit.MongoDB.Providers;
using Audit.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Econolite.Ode.Auditing.Extensions;

public static class Auditing
{
    public static IServiceCollection AddAudit(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuditScopeFactory, AuditScopeFactory>();
        services.AddScoped<IAuditCrudScopeFactory, AuditCrudScopeFactory>();
        return services;
    }

    public static WebApplication UseAudit(this WebApplication app, IHttpContextAccessor httpContextAccessor)
    {
        app.UseAuditMiddleware(_ => _
            .FilterByRequest(rq => rq.Method != "GET")
            .WithEventType("{verb}:{url}")
            .IncludeHeaders()
            .IncludeResponseHeaders()
            .IncludeRequestBody()
            .IncludeResponseBody());

        Audit.Core.Configuration.AddCustomAction(ActionType.OnEventSaving, scope =>
        {
            var auditAction = scope.Event.GetWebApiAuditAction();
            if (auditAction == null)
            {
                return;
            }

            // Removing sensitive headers
            auditAction.Headers.Remove("Authorization");

            var claim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "name");
            // Adding custom details to the log
            scope.Event.CustomFields.Add("User", claim?.Value ?? "unknown");
            // Removing request body conditionally as an example
            if (auditAction.HttpMethod.Equals("DELETE"))
            {
                auditAction.RequestBody = null;
            }
        });

        return app;
    }


}

public interface IAuditCrudScopeFactory
{
    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    IAuditScope CreateAdd(string type, Func<object> targetGetter);

    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    /// <param name="targetGetter"></param>
    /// <param name="cancellationToken">The Cancellation Token.</param>
    /// <param name="type"></param>
    Task<IAuditScope> CreateAddAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    IAuditScope CreateUpdate(string type, Func<object> targetGetter);

    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetGetter"></param>
    /// <param name="cancellationToken">The Cancellation Token.</param>
    Task<IAuditScope> CreateUpdateAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetGetter"></param>
    IAuditScope CreateDelete(string type, Func<object> targetGetter);

    /// <summary>
    /// Creates an audit scope with the given creation options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetGetter"></param>
    /// <param name="cancellationToken">The Cancellation Token.</param>
    Task<IAuditScope> CreateDeleteAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default);
}

public class AuditCrudScopeFactory : IAuditCrudScopeFactory
{
    private readonly IAuditScopeFactory _auditScopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditCrudScopeFactory(IAuditScopeFactory auditScopeFactory, IHttpContextAccessor httpContextAccessor)
    {
        _auditScopeFactory = auditScopeFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public IAuditScope CreateAdd(string type, Func<object> targetGetter)
    {
        var username = GetUsername();
        return _auditScopeFactory.Create(new AuditScopeOptions($"{type}:Add", targetGetter, new { Username = username }, null, null, true));
    }

    public Task<IAuditScope> CreateAddAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default)
    {
        var username = GetUsername();
        return _auditScopeFactory.CreateAsync(new AuditScopeOptions($"{type}:Add", targetGetter, new { Username = username }, null, null, true), cancellationToken);
    }

    public IAuditScope CreateUpdate(string type, Func<object> targetGetter)
    {
        var username = GetUsername();
        return _auditScopeFactory.Create(new AuditScopeOptions($"{type}:Update", targetGetter, new { Username = username }));
    }

    public Task<IAuditScope> CreateUpdateAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default)
    {
        var username = GetUsername();
        return _auditScopeFactory.CreateAsync(new AuditScopeOptions($"{type}:Update", targetGetter, new { Username = username }), cancellationToken);
    }

    public IAuditScope CreateDelete(string type, Func<object> targetGetter)
    {
        var username = GetUsername();
        return _auditScopeFactory.Create(new AuditScopeOptions($"{type}:Delete", targetGetter, new { Username = username }));
    }

    public Task<IAuditScope> CreateDeleteAsync(string type, Func<object> targetGetter, CancellationToken cancellationToken = default)
    {
        var username = GetUsername();
        return _auditScopeFactory.CreateAsync(new AuditScopeOptions($"{type}:Delete", targetGetter, new { Username = username }), cancellationToken);
    }

    private string GetUsername()
    {
        var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "preferred_username");
        return claim?.Value ?? "Unknown";
    }
}

public class AuditMongoDataProvider : MongoDataProvider
{
    public AuditMongoDataProvider(Action<IMongoProviderConfigurator> config) : base(config)
    {
        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || true);
        BsonSerializer.RegisterSerializer(objectSerializer);
    }

    public override object? Serialize<T>(T value)
    {
        if (value is null or string or BsonDocument)
        {
            return value;
        }
        if (BsonTypeMapper.TryMapToBsonValue(value, out BsonValue bsonValue))
        {
            return bsonValue;
        }
        return value.ToBsonDocument(typeof(object));
    }
}
