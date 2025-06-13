using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents analysis in a vCon.
/// </summary>
public class Analysis
{
    /// <summary>
    /// Gets or sets the type of analysis.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets reference to the dialog(s) that were analyzed.
    /// </summary>
    [JsonPropertyName("dialog")]
    public object Dialog { get; set; } = default!; // Can be int or int[]

    /// <summary>
    /// Gets or sets the vendor that performed the analysis.
    /// </summary>
    [JsonPropertyName("vendor")]
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the analysis content.
    /// </summary>
    [JsonPropertyName("body")]
    public object? Body { get; set; }

    /// <summary>
    /// Gets or sets the encoding used for the analysis content.
    /// </summary>
    [JsonPropertyName("encoding")]
    public Encoding? Encoding { get; set; }

    /// <summary>
    /// Gets or sets additional properties.
    /// </summary>
    [JsonPropertyName("extra")]
    public Dictionary<string, object>? Extra { get; set; }
}