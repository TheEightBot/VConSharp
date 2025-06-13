using System.Text.Json.Serialization;

namespace VConSharp;

/// <summary>
/// Represents a party in a vCon.
/// </summary>
public class Party
{
    /// <summary>
    /// Gets or sets the telephone number of the party.
    /// </summary>
    [JsonPropertyName("tel")]
    public string? Tel { get; set; }

    /// <summary>
    /// Gets or sets the STIR identifier of the party.
    /// </summary>
    [JsonPropertyName("stir")]
    public string? Stir { get; set; }

    /// <summary>
    /// Gets or sets the email address of the party.
    /// </summary>
    [JsonPropertyName("mailto")]
    public string? Mailto { get; set; }

    /// <summary>
    /// Gets or sets the name of the party.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the validation information of the party.
    /// </summary>
    [JsonPropertyName("validation")]
    public string? Validation { get; set; }

    /// <summary>
    /// Gets or sets the GML position of the party.
    /// </summary>
    [JsonPropertyName("gmlpos")]
    public string? Gmlpos { get; set; }

    /// <summary>
    /// Gets or sets the civic address of the party.
    /// </summary>
    [JsonPropertyName("civicaddress")]
    public CivicAddress? CivicAddress { get; set; }

    /// <summary>
    /// Gets or sets the UUID of the party.
    /// </summary>
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    /// <summary>
    /// Gets or sets the role of the party.
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the contact list of the party.
    /// </summary>
    [JsonPropertyName("contact_list")]
    public string? ContactList { get; set; }

    /// <summary>
    /// Gets or sets additional metadata of the party.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }

    /// <summary>
    /// Gets or sets additional custom properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Party"/> class.
    /// Creates a new instance of Party.
    /// </summary>
    public Party()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Party"/> class.
    /// Creates a new instance of Party with the specified parameters.
    /// </summary>
    /// <param name="parameters">Dictionary of party parameters.</param>
    public Party(Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            switch (param.Key)
            {
                case "tel":
                    Tel = param.Value.ToString();
                    break;
                case "stir":
                    Stir = param.Value.ToString();
                    break;
                case "mailto":
                    Mailto = param.Value.ToString();
                    break;
                case "name":
                    Name = param.Value.ToString();
                    break;
                case "validation":
                    Validation = param.Value.ToString();
                    break;
                case "gmlpos":
                    Gmlpos = param.Value.ToString();
                    break;
                case "civicaddress":
                    CivicAddress = param.Value as CivicAddress;
                    break;
                case "uuid":
                    Uuid = param.Value.ToString();
                    break;
                case "role":
                    Role = param.Value.ToString();
                    break;
                case "contact_list":
                    ContactList = param.Value.ToString();
                    break;
                case "meta":
                    Meta = param.Value as Dictionary<string, object>;
                    break;
                default:
                    if (AdditionalProperties == null)
                    {
                        AdditionalProperties = new Dictionary<string, object>();
                    }

                    AdditionalProperties[param.Key] = param.Value;
                    break;
            }
        }
    }

    /// <summary>
    /// Converts the party to a dictionary representation.
    /// </summary>
    public Dictionary<string, object?> ToDict()
    {
        var dict = new Dictionary<string, object?>();

        if (Tel != null)
        {
            dict["tel"] = Tel;
        }

        if (Stir != null)
        {
            dict["stir"] = Stir;
        }

        if (Mailto != null)
        {
            dict["mailto"] = Mailto;
        }

        if (Name != null)
        {
            dict["name"] = Name;
        }

        if (Validation != null)
        {
            dict["validation"] = Validation;
        }

        if (Gmlpos != null)
        {
            dict["gmlpos"] = Gmlpos;
        }

        if (CivicAddress != null)
        {
            dict["civicaddress"] = CivicAddress;
        }

        if (Uuid != null)
        {
            dict["uuid"] = Uuid;
        }

        if (Role != null)
        {
            dict["role"] = Role;
        }

        if (ContactList != null)
        {
            dict["contact_list"] = ContactList;
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
}