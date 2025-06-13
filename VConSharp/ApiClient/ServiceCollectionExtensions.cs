using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace VConSharp.ApiClient;

/// <summary>
/// Extension methods for registering VConSharp API client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the VConApiClient to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure the client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddVConApiClient(
        this IServiceCollection services,
        Action<VConApiClientOptions> configureOptions)
    {
        // Add the client options
        var options = new VConApiClientOptions();
        configureOptions(options);

        services.AddSingleton(options);

        // Add the named HTTP client
        services.AddHttpClient<IVConApiClient, VConApiClient>((provider, client) =>
        {
            // Configure base address and default headers in the VConApiClient constructor
        });

        // Register the client as a singleton
        services.AddSingleton<IVConApiClient, VConApiClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(VConApiClient));
            var clientOptions = provider.GetRequiredService<VConApiClientOptions>();
            var logger = provider.GetService<ILogger<VConApiClient>>();

            return new VConApiClient(httpClient, clientOptions, logger);
        });

        return services;
    }
}
