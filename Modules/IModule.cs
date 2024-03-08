// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Modules;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection builder);
    IEndpointRouteBuilder MapEndpoints(WebApplication endpoints);
}
 
public static class ModuleExtensions
{
    // this could also be added into the DI container
    static readonly List<IModule> RegisteredModules = new List<IModule>();
 
    public static IServiceCollection RegisterModules(this IServiceCollection services, Type type)
    {
        var modules = DiscoverModules(type);
        foreach (var module in modules)
        {
            module.RegisterModule(services);
            RegisteredModules.Add(module);
        }
 
        return services;
    }
 
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in RegisteredModules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }
 
    private static IEnumerable<IModule> DiscoverModules(Type type)
    {
        return type.Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
