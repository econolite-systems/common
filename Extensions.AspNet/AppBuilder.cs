// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Common.Extensions;
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.HealthChecks.Kafka.Extensions;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Econolite.Ode.Extensions.AspNet;

public static class AppBuilder
{
    public record AppBuilderOptions
    {
        public string Source = string.Empty;
        public Monitoring.Events.LogName DefaultLogName = Monitoring.Events.LogName.SystemEvent;
        public Monitoring.Events.Category DefaultCategory = Monitoring.Events.Category.Server;
        public Guid DefaultTenantId = Guid.Empty;
        // If it gets tedious adding the authorization reference, can split the Auth sections to be part of an injected "modules" setup?
        public bool IsApi;
        public bool IncludeXmlComments;
        public string[]? CorsOrigins { get; set; }
    }

    public static WebApplicationBuilder CreateWebApplication(string[] args, Action<AppBuilderOptions> appOptions, Action<WebApplicationBuilder, IServiceCollection> customServices, Action<WebApplicationBuilder, IHealthChecksBuilder>? healthCheckAdditions = null)
    {
        var appBuilderOptions = new AppBuilderOptions();
        appOptions(appBuilderOptions);
        return CreateWebApplication(args, appBuilderOptions, customServices, healthCheckAdditions);
    }

    public const string AllOrigins = "_allOrigins";
    public static WebApplicationBuilder CreateWebApplication(string[] args, AppBuilderOptions appBuilderOptions, Action<WebApplicationBuilder, IServiceCollection> customServices, Action<WebApplicationBuilder, IHealthChecksBuilder>? healthCheckAdditions = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddOdeLogging();

        builder.Services.AddMessaging();

        builder.Services.AddMetrics(builder.Configuration, appBuilderOptions.Source)
                    .AddUserEventSupport(builder.Configuration, _ =>
                    {
                        _.DefaultSource = appBuilderOptions.Source;
                        _.DefaultLogName = appBuilderOptions.DefaultLogName;
                        _.DefaultCategory = appBuilderOptions.DefaultCategory;
                        _.DefaultTenantId = appBuilderOptions.DefaultTenantId;
                    });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllOrigins,
                policy =>
                {
                    var corPolicyBuilder = policy.AllowAnyHeader().AllowAnyMethod();
                    if (appBuilderOptions.CorsOrigins?.Length > 0)
                    {
                        corPolicyBuilder.WithOrigins(appBuilderOptions.CorsOrigins);
                        
                    }
                    else
                    {
                        // AllowAnyOrigin isn't allowed with AllowCredentials.
                        // corPolicyBuilder.AllowAnyOrigin();
                        // This will do the same thing as AllowAnyOrigin.
                        corPolicyBuilder.SetIsOriginAllowed(origin => true);
                    }
                    corPolicyBuilder.AllowCredentials();
                });
        });

        if (appBuilderOptions.IsApi)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddMvc(config => { config.Filters.Add(new AuthorizeFilter()); });

            builder.Services.AddAuthenticationJwtBearer(builder.Configuration, builder.Environment.IsDevelopment());

            builder.Services.AddSwaggerGen(c =>
            {
#if DEBUG
                if (appBuilderOptions.IncludeXmlComments)
                {
                    var basePath = AppDomain.CurrentDomain.BaseDirectory;
                    var fileName = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name + ".xml";
                    c.IncludeXmlComments(Path.Combine(basePath, fileName));
                }
#endif

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme,
                            },
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });
            });
        }

        customServices(builder, builder.Services);

        var healthChecksBuilder = builder.Services.AddHealthChecks()
            .AddProcessAllocatedMemoryHealthCheck(maximumMegabytesAllocated: 1024, name: "Process Allocated Memory", tags: new[] { "memory" })
            .AddKafkaHealthCheck();
        healthCheckAdditions?.Invoke(builder, healthChecksBuilder);

        return builder;
    }

    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder, bool isApi = false, Action<WebApplication>? preAuthApplicationAdditions = null, Action<WebApplication>? postAuthApplicationAdditions = null)
    {
        var app = builder.Build();
        if (isApi)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(AllOrigins);
        app.UseRouting();
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        preAuthApplicationAdditions?.Invoke(app);

        if (isApi)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.AddUnhandledExceptionLogging();
        app.UseHealthChecksPrometheusExporter("/metrics");

        postAuthApplicationAdditions?.Invoke(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
            endpoints.MapControllers();
        });

        app.LogStartup();

        return app;
    }

    public static WebApplication CreateAndBuildWebApplication(string[] args, Action<AppBuilderOptions> appOptions, Action<WebApplicationBuilder, IServiceCollection> customServices, Action<WebApplicationBuilder, IHealthChecksBuilder>? healthCheckAdditions = null, Action<WebApplication>? preAuthApplicationAdditions = null, Action<WebApplication>? postAuthApplicationAdditions = null)
    {
        var appBuilderOptions = new AppBuilderOptions();
        appOptions(appBuilderOptions);

        var builder = CreateWebApplication(args, appBuilderOptions, customServices, healthCheckAdditions);
        var app = builder.BuildWebApplication(appBuilderOptions.IsApi, preAuthApplicationAdditions, postAuthApplicationAdditions);
        return app;
    }

    public static Task BuildAndRunWebHostAsync(string[] args, Action<AppBuilderOptions> appOptions, Action<WebApplicationBuilder, IServiceCollection> customServices, Action<WebApplicationBuilder, IHealthChecksBuilder>? healthCheckAdditions = null, Action<WebApplication>? postAuthApplicationAdditions = null)
    {
        var appBuilderOptions = new AppBuilderOptions();
        appOptions(appBuilderOptions);

        var builder = CreateWebApplication(args, appBuilderOptions, customServices, healthCheckAdditions);
        // preAuthApplicationAdditions null until revisiting in services already using these extensions
        var app = builder.BuildWebApplication(appBuilderOptions.IsApi, preAuthApplicationAdditions: null, postAuthApplicationAdditions);
        return app.RunAsync();
    }
}
