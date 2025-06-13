using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents an attachment in a vCon.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Gets or sets the MIME type of the attachment.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the attachment.
    /// </summary>
    [JsonPropertyName("body")]
    public object? Body { get; set; }

    /// <summary>
    /// Gets or sets the encoding used for the attachment content.
    /// </summary>
    [JsonPropertyName("encoding")]
    public Encoding Encoding { get; set; } = Encoding.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="Attachment"/> class.
    /// Creates a new instance of Attachment with specified parameters.
    /// </summary>
    public Attachment(string type, object body, Encoding encoding = Encoding.None)
    {
        this.Type = type;
        this.Body = body;
        this.Encoding = encoding;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attachment"/> class.
    /// Creates a new instance of Attachment.
    /// </summary>
    public Attachment()
    {
    }

    /// <summary>
    /// Converts the attachment to a dictionary representation.
    /// </summary>
    public Dictionary<string, object?> ToDict()
    {
        return new Dictionary<string, object?>
        {
            ["type"] = this.Type,
            ["body"] = this.Body,
            ["encoding"] = this.Encoding.ToString().ToLowerInvariant(),
        };
    }
}
