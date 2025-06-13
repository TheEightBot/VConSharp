using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents a civic address in a vCon.
/// </summary>
public class CivicAddress
{
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locality (city).
    /// </summary>
    [JsonPropertyName("locality")]
    public string Locality { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region (state/province).
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string Postcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;
}
