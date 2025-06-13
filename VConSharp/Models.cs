using System.Text.Json.Serialization;

namespace VConSharp
{
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
            Type = type;
            Body = body;
            Encoding = encoding;
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
                ["type"] = Type,
                ["body"] = Body,
                ["encoding"] = Encoding.ToString().ToLowerInvariant(),
            };
        }
    }

    /// <summary>
    /// Represents a party history event in a vCon.
    /// </summary>
    public class PartyHistory
    {
        /// <summary>
        /// Gets or sets the index of the party in the parties array.
        /// </summary>
        [JsonPropertyName("party")]
        public int Party { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time when the event occurred.
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyHistory"/> class.
        /// Creates a new instance of PartyHistory.
        /// </summary>
        public PartyHistory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyHistory"/> class.
        /// Creates a new instance of PartyHistory with specified parameters.
        /// </summary>
        public PartyHistory(int party, string @event, DateTime time)
        {
            Party = party;
            Event = @event;
            Time = time;
        }

        /// <summary>
        /// Converts the party history to a dictionary representation.
        /// </summary>
        public Dictionary<string, object> ToDict()
        {
            return new Dictionary<string, object>
            {
                ["party"] = Party,
                ["event"] = Event,
                ["time"] = Time.ToString("o"),
            };
        }
    }

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
}
