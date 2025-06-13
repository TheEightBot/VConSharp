using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents a dialog in a vCon.
/// </summary>
public class Dialog
{
    /// <summary>
    /// List of supported MIME types for dialogs.
    /// </summary>
    public static readonly string[] MimeTypes = new[]
    {
        "text/plain",
        "audio/x-wav",
        "audio/wav",
        "audio/wave",
        "audio/mpeg",
        "audio/mp3",
        "audio/ogg",
        "audio/webm",
        "audio/x-m4a",
        "audio/aac",
        "video/mp4",
        "video/x-mp4",
        "video/ogg",
        "multipart/mixed",
        "message/rfc822",
    };

    /// <summary>
    /// Gets or sets the type of dialog.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start time of the dialog.
    /// </summary>
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    /// <summary>
    /// Gets or sets the parties involved in the dialog.
    /// </summary>
    [JsonPropertyName("parties")]
    public List<int> Parties { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets the party that originated the dialog.
    /// </summary>
    [JsonPropertyName("originator")]
    public int? Originator { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the dialog content.
    /// </summary>
    [JsonPropertyName("mimetype")]
    public string? Mimetype { get; set; }

    /// <summary>
    /// Gets or sets the filename of the dialog content.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets the content of the dialog.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the encoding of the dialog content.
    /// </summary>
    [JsonPropertyName("encoding")]
    public string? Encoding { get; set; }

    /// <summary>
    /// Gets or sets the URL to the external dialog content.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the algorithm used for the signature.
    /// </summary>
    [JsonPropertyName("alg")]
    public string? Alg { get; set; }

    /// <summary>
    /// Gets or sets the signature of the dialog content.
    /// </summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>
    /// Gets or sets the disposition of the dialog.
    /// </summary>
    [JsonPropertyName("disposition")]
    public string? Disposition { get; set; }

    /// <summary>
    /// Gets or sets the history of party events in the dialog.
    /// </summary>
    [JsonPropertyName("party_history")]
    public List<PartyHistory>? PartyHistory { get; set; }

    /// <summary>
    /// Gets or sets the party that was transferred.
    /// </summary>
    [JsonPropertyName("transferee")]
    public int? Transferee { get; set; }

    /// <summary>
    /// Gets or sets the party that initiated the transfer.
    /// </summary>
    [JsonPropertyName("transferor")]
    public int? Transferor { get; set; }

    /// <summary>
    /// Gets or sets the target of the transfer.
    /// </summary>
    [JsonPropertyName("transfer_target")]
    public int? TransferTarget { get; set; }

    /// <summary>
    /// Gets or sets the original dialog index.
    /// </summary>
    [JsonPropertyName("original")]
    public int? Original { get; set; }

    /// <summary>
    /// Gets or sets the consultation dialog index.
    /// </summary>
    [JsonPropertyName("consultation")]
    public int? Consultation { get; set; }

    /// <summary>
    /// Gets or sets the target dialog index.
    /// </summary>
    [JsonPropertyName("target_dialog")]
    public int? TargetDialog { get; set; }

    /// <summary>
    /// Gets or sets the campaign associated with the dialog.
    /// </summary>
    [JsonPropertyName("campaign")]
    public string? Campaign { get; set; }

    /// <summary>
    /// Gets or sets the interaction type of the dialog.
    /// </summary>
    [JsonPropertyName("interaction")]
    public string? Interaction { get; set; }

    /// <summary>
    /// Gets or sets the skill associated with the dialog.
    /// </summary>
    [JsonPropertyName("skill")]
    public string? Skill { get; set; }

    /// <summary>
    /// Gets or sets the duration of the dialog in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the dialog.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }

    /// <summary>
    /// Gets or sets additional custom properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dialog"/> class.
    /// Creates a new instance of Dialog.
    /// </summary>
    public Dialog()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dialog"/> class.
    /// Creates a new instance of Dialog with required parameters.
    /// </summary>
    public Dialog(string type, DateTime start, IEnumerable<int> parties)
    {
        Type = type;
        Start = start;
        Parties = parties.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dialog"/> class.
    /// Creates a new instance of Dialog with parameters dictionary.
    /// </summary>
    public Dialog(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("type", out var type) || type == null)
        {
            throw new ArgumentException("Type is required", nameof(parameters));
        }

        Type = type.ToString()!;

        if (!parameters.TryGetValue("start", out var start) || start == null)
        {
            throw new ArgumentException("Start is required", nameof(parameters));
        }

        Start = start is DateTime dateTime ? dateTime : DateTime.Parse(start.ToString()!);

        if (!parameters.TryGetValue("parties", out var parties) || parties == null)
        {
            throw new ArgumentException("Parties is required", nameof(parameters));
        }

        if (parties is List<int> partiesList)
        {
            Parties = partiesList;
        }
        else if (parties is int[] partiesArray)
        {
            Parties = partiesArray.ToList();
        }
        else if (parties is IEnumerable<int> partiesEnum)
        {
            Parties = partiesEnum.ToList();
        }

        // Set other properties if provided
        if (parameters.TryGetValue("originator", out var originator) && originator != null)
        {
            Originator = Convert.ToInt32(originator);
        }

        if (parameters.TryGetValue("mimetype", out var mimetype) && mimetype != null)
        {
            Mimetype = mimetype.ToString();
        }

        if (parameters.TryGetValue("filename", out var filename) && filename != null)
        {
            Filename = filename.ToString();
        }

        if (parameters.TryGetValue("body", out var body) && body != null)
        {
            Body = body.ToString();
        }

        if (parameters.TryGetValue("encoding", out var encoding) && encoding != null)
        {
            Encoding = encoding.ToString();
        }

        if (parameters.TryGetValue("url", out var url) && url != null)
        {
            Url = url.ToString();
        }

        if (parameters.TryGetValue("duration", out var duration) && duration != null)
        {
            Duration = Convert.ToInt32(duration);
        }
    }

    /// <summary>
    /// Converts the dialog to a dictionary representation.
    /// </summary>
    public Dictionary<string, object?> ToDict()
    {
        var dict = new Dictionary<string, object?>
        {
            ["type"] = Type,
            ["start"] = Start,
            ["parties"] = Parties,
        };

        if (Originator.HasValue)
        {
            dict["originator"] = Originator.Value;
        }

        if (Mimetype != null)
        {
            dict["mimetype"] = Mimetype;
        }

        if (Filename != null)
        {
            dict["filename"] = Filename;
        }

        if (Body != null)
        {
            dict["body"] = Body;
        }

        if (Encoding != null)
        {
            dict["encoding"] = Encoding;
        }

        if (Url != null)
        {
            dict["url"] = Url;
        }

        if (Alg != null)
        {
            dict["alg"] = Alg;
        }

        if (Signature != null)
        {
            dict["signature"] = Signature;
        }

        if (Disposition != null)
        {
            dict["disposition"] = Disposition;
        }

        if (PartyHistory != null)
        {
            dict["party_history"] = PartyHistory;
        }

        if (Transferee.HasValue)
        {
            dict["transferee"] = Transferee.Value;
        }

        if (Transferor.HasValue)
        {
            dict["transferor"] = Transferor.Value;
        }

        if (TransferTarget.HasValue)
        {
            dict["transfer_target"] = TransferTarget.Value;
        }

        if (Original.HasValue)
        {
            dict["original"] = Original.Value;
        }

        if (Consultation.HasValue)
        {
            dict["consultation"] = Consultation.Value;
        }

        if (TargetDialog.HasValue)
        {
            dict["target_dialog"] = TargetDialog.Value;
        }

        if (Campaign != null)
        {
            dict["campaign"] = Campaign;
        }

        if (Interaction != null)
        {
            dict["interaction"] = Interaction;
        }

        if (Skill != null)
        {
            dict["skill"] = Skill;
        }

        if (Duration.HasValue)
        {
            dict["duration"] = Duration.Value;
        }

        if (Meta != null)
        {
            dict["meta"] = Meta;
        }

        if (AdditionalProperties != null)
        {
            foreach (var prop in AdditionalProperties)
            {
                dict[prop.Key] = prop.Value;
            }
        }

        return dict;
    }

    /// <summary>
    /// Adds external data to the dialog.
    /// </summary>
    public void AddExternalData(string url, string filename, string mimetype)
    {
        if (!MimeTypes.Contains(mimetype))
        {
            throw new ArgumentException($"Invalid MIME type: {mimetype}");
        }

        Url = url;
        Filename = filename;
        Mimetype = mimetype;
        Body = null;
        Encoding = null;
    }

    /// <summary>
    /// Adds inline data to the dialog.
    /// </summary>
    public void AddInlineData(string body, string filename, string mimetype)
    {
        if (!MimeTypes.Contains(mimetype))
        {
            throw new ArgumentException($"Invalid MIME type: {mimetype}");
        }

        Body = body;
        Filename = filename;
        Mimetype = mimetype;
        Url = null;
    }

    /// <summary>
    /// Checks if the dialog has external data.
    /// </summary>
    public bool IsExternalData() => Url != null;

    /// <summary>
    /// Checks if the dialog has inline data.
    /// </summary>
    public bool IsInlineData() => Body != null;

    /// <summary>
    /// Checks if the dialog is a text dialog.
    /// </summary>
    public bool IsText() => Mimetype == "text/plain";

    /// <summary>
    /// Checks if the dialog is an audio dialog.
    /// </summary>
    public bool IsAudio() => new[]
    {
        "audio/x-wav",
        "audio/wav",
        "audio/wave",
        "audio/mpeg",
        "audio/mp3",
        "audio/ogg",
        "audio/webm",
        "audio/x-m4a",
        "audio/aac",
    }.Contains(Mimetype);

    /// <summary>
    /// Checks if the dialog is a video dialog.
    /// </summary>
    public bool IsVideo() => new[]
    {
        "video/mp4",
        "video/x-mp4",
        "video/ogg",
    }.Contains(Mimetype);

    /// <summary>
    /// Checks if the dialog is an email.
    /// </summary>
    public bool IsEmail() => Mimetype == "message/rfc822";
}