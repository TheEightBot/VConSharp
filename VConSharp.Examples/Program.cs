using Microsoft.Extensions.Configuration;

namespace VConSharp.Examples
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("VConSharp Examples");
            Console.WriteLine("=================");
            Console.WriteLine();

            // Run simple example
            SimpleExample.Run();

            // Temporarily comment out other examples for testing
            // Console.WriteLine();
            // AudioVideoExample.Run();

            // Load configuration from secrets.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(ApiClientExample).Assembly) // Using assembly instead of static class
                .AddCommandLine(args)
                .Build();

            // Get API settings from configuration
            var rootUrl = configuration["root_url"];
            var apiToken = configuration["api_token"];

            // Run API client example if configuration is available
            if (!string.IsNullOrEmpty(rootUrl) && !string.IsNullOrEmpty(apiToken))
            {
                Console.WriteLine();
                Console.WriteLine($"Running API client example with URL: {rootUrl}");
                await ApiClientExample.RunAsync(rootUrl, apiToken);
            }

            // If command line arguments are provided, use those instead
            else if (args.Length >= 2)
            {
                Console.WriteLine();
                Console.WriteLine($"Running API client example with URL: {args[0]}");
                await ApiClientExample.RunAsync(args[0], args[1]);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("API client example not run. Missing configuration.");
                Console.WriteLine("To run the API client example, either:");
                Console.WriteLine("1. Add user secrets using: dotnet user-secrets set \"root_url\" \"https://api-url\"");
                Console.WriteLine("2. Add user secrets using: dotnet user-secrets set \"api_token\" \"your-token\"");
                Console.WriteLine("   OR");
                Console.WriteLine("3. Provide as command line arguments: dotnet run --project VConSharp.Examples/VConSharp.Examples.csproj https://api-url x-api-token");
            }
        }
    }
}
