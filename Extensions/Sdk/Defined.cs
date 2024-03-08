using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Econolite.Ode.Extensions.Sdk;

public static class Defined
{
    public const string ClientName = "SdkClient";

    private static readonly TimeSpan[] SleepDurations = new[]
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    };

    public static IServiceCollection AddRetryHttpClient<TClient>(this IServiceCollection services, int timeoutInSeconds = 30) where TClient : class
    {
        services.AddHttpClient<TClient>().ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(SleepDurations));
        return services;
    }
    
    public static IServiceCollection AddRetryHttpClient(this IServiceCollection services, string clientName, int timeoutInSeconds = 30)
    {
        services.AddHttpClient(clientName).ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(SleepDurations));
        return services;
    }
}