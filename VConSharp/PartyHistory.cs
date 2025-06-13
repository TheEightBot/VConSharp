using System.Text.Json.Serialization;

namespace VConSharp;

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
        this.Party = party;
        this.Event = @event;
        this.Time = time;
    }

    /// <summary>
    /// Converts the party history to a dictionary representation.
    /// </summary>
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            ["party"] = this.Party,
            ["event"] = this.Event,
            ["time"] = this.Time.ToString("o"),
        };
    }
}
