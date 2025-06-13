namespace VConSharp.ApiClient;

/// <summary>
/// Options for configuring the VConApiClient.
/// </summary>
public class VConApiClientOptions
{
    /// <summary>
    /// Gets or sets the base URL of the vCon API.
    /// </summary>
    /// <remarks>
    /// This should include the full base URL including the scheme and host, for example:
    /// https://example.com/api.
    /// </remarks>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API token for authentication.
    /// </summary>
    public string? ApiToken { get; set; }
}
