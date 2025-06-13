using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VConSharp.ApiClient;

namespace VConSharp.Examples;

/// <summary>
/// Example showing how to use the VConSharp API client.
/// </summary>
public static class ApiClientExample
{
    /// <summary>
    /// Runs the API client example.
    /// </summary>
    /// <param name="apiUrl">The base URL of the API.</param>
    /// <param name="apiToken">The API token for authentication.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunAsync(string apiUrl, string apiToken)
    {
        // Set up dependency injection
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Register the VCon API client
        services.AddVConApiClient(options =>
        {
            options.BaseUrl = apiUrl;
            options.ApiToken = apiToken;
        });

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get the API client from the service provider
        var apiClient = serviceProvider.GetRequiredService<IVConApiClient>();

        Console.WriteLine("VConSharp API Client Example");
        Console.WriteLine("============================");

        try
        {
            // Get a list of vCons
            Console.WriteLine("Getting a list of vCon UUIDs...");
            var vconUuids = await apiClient.GetVConsUuidsAsync();
            Console.WriteLine($"Found {vconUuids.Count} vCons");

            if (vconUuids.Count > 0)
            {
                // Get the first vCon from the list
                var firstUuid = Guid.Parse(vconUuids.First());
                Console.WriteLine($"Getting vCon with UUID: {firstUuid}");
                var vcon = await apiClient.GetVConAsync(firstUuid);

                if (vcon != null)
                {
                    Console.WriteLine($"Successfully retrieved vCon: {vcon.Subject ?? "No subject"}");
                    Console.WriteLine($"Created at: {vcon.CreatedAt}");
                    Console.WriteLine($"Parties: {vcon.Parties.Count}");
                    Console.WriteLine($"Dialogs: {vcon.Dialog?.Count ?? 0}");
                }
            }

            // Create a new vCon
            Console.WriteLine("\nCreating a new vCon...");
            var newVCon = new VCon();

            // Set the vCon property (required by the API)
            newVCon.Vcon = "1.0";
            newVCon.Subject = "Test conversation";
            newVCon.AddParty(new Party { Name = "John Doe", Tel = "+1234567890", Mailto = "john@example.com", });
            newVCon.AddParty(new Party { Name = "Jane Smith", Tel = "+1987654321", Mailto = "jane@example.com", });

            // Add a text dialog
            newVCon.AddDialog(new Dialog
            {
                Type = "text",
                Start = DateTime.UtcNow,
                Parties = new List<int> { 0, },
                Body = "Hello, this is John!",
            });

            newVCon.AddDialog(new Dialog
            {
                Type = "text",
                Start = DateTime.UtcNow.AddMinutes(1),
                Parties = new List<int> { 1, },
                Body = "Hi John, nice to meet you!",
            });

            // Save the vCon via the API
            var createdVCon = await apiClient.CreateVConAsync(newVCon);
            Console.WriteLine($"Created new vCon with UUID: {createdVCon.Uuid}");

            // Search for vCons by party information
            Console.WriteLine("\nSearching for vCons...");
            var searchResults = await apiClient.SearchVConsAsync(tel: "+1234567890");
            Console.WriteLine($"Found {searchResults.Count} vCons matching the search criteria");

            // Clean up - delete the vCon we created
            Console.WriteLine("\nDeleting the newly created vCon...");
            await apiClient.DeleteVConAsync(Guid.Parse(createdVCon.Uuid));
            Console.WriteLine("vCon deleted successfully");

            // Get system configuration
            Console.WriteLine("\nGetting system configuration...");
            var config = await apiClient.GetConfigAsync();
            Console.WriteLine($"System configuration contains {config.Count} settings");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }
}
