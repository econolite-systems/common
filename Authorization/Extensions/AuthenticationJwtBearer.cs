// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Econolite.Ode.Authorization.Extensions
{
    public static class AuthenticationJwtBearer
    {
        /// <summary>
        /// Requires Authentication:Authority in config, will also support legacy connections via Authentication:LegacyAuthority
        /// </summary>
        /// <param name="services">Appends AddAuthentication with AddJwtBearer.</param>
        /// <param name="configuration">Authentication:Authority required. Authentication:LegacyAuthority optional.</param>
        /// <param name="isDevelopment">Toggles if Https metadata is required.</param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationJwtBearer(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            var supportLegacy = !string.IsNullOrWhiteSpace(configuration["Authentication:LegacyAuthority"]);
            var auth = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.Authority = configuration["Authentication:Authority"];
                options.RequireHttpsMetadata = !isDevelopment;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = configuration["Authentication:Authority"],
                };

                // Don't double log if both are being handled.
                if (!supportLegacy)
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is not SecurityTokenExpiredException)
                            {
                                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                                var logger = loggerFactory.CreateLogger("JwtBearer");
                                logger.LogError(context.Exception, "Error validating JWT token");
                            }

                            return Task.CompletedTask;
                        },
                    };
                }
            });

            if (supportLegacy)
            {
                auth.AddJwtBearer("Legacy", options =>
                {
                    options.SaveToken = true;
                    options.Authority = configuration["Authentication:LegacyAuthority"];
                    options.RequireHttpsMetadata = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is not SecurityTokenExpiredException)
                            {
                                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                                var logger = loggerFactory.CreateLogger("JwtBearer");
                                logger.LogError(context.Exception, "Error validating JWT token");
                            }

                            return Task.CompletedTask;
                        },
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidIssuer = configuration["Authentication:LegacyAuthority"],
                    };
                });
            }

            return services;
        }
    }
}
