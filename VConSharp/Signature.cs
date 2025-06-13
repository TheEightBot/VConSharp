using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents a signature in a vCon.
/// </summary>
public class Signature
{
    /// <summary>
    /// Gets or sets the header of the JWS signature.
    /// </summary>
    [JsonPropertyName("header")]
    public Dictionary<string, string>? Header { get; set; }

    /// <summary>
    /// Gets or sets the JWS signature.
    /// </summary>
    [JsonPropertyName("signature")]
    public string JwsSignature { get; set; } = string.Empty;
}