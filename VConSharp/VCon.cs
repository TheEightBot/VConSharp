using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace VConSharp;

/// <summary>
/// The main vCon class for managing virtual conversations.
/// </summary>
public class VCon
{
    /// <summary>
    /// Logger for the VCon class.
    /// </summary>
    private readonly ILogger<VCon> _logger;

    /// <summary>
    /// Gets or sets the UUID of the vCon.
    /// </summary>
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    /// <summary>
    /// Gets or sets the vCon specification version.
    /// This is a required field when sending vCons to the API.
    /// </summary>
    [JsonPropertyName("vcon")]
    public string? Vcon { get; set; }

    /// <summary>
    /// Gets or sets the subject of the vCon.
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the vCon.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update time of the vCon.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets redacted content in the vCon.
    /// </summary>
    [JsonPropertyName("redacted")]
    public Dictionary<string, object>? RedactedContent { get; set; }

    /// <summary>
    /// Gets or sets appended content in the vCon.
    /// </summary>
    [JsonPropertyName("appended")]
    public Dictionary<string, object>? AppendedContent { get; set; }

    /// <summary>
    /// Gets a value indicating whether the vCon has been redacted.
    /// </summary>
    [JsonIgnore]
    public bool Redacted => RedactedContent != null && RedactedContent.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the vCon has been appended.
    /// </summary>
    [JsonIgnore]
    public bool Appended => AppendedContent != null && AppendedContent.Count > 0;

    /// <summary>
    /// Gets or sets the group of the vCon.
    /// </summary>
    [JsonPropertyName("group")]
    public List<string>? Group { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the vCon.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; set; }

    /// <summary>
    /// Gets or sets the parties in the vCon.
    /// </summary>
    [JsonPropertyName("parties")]
    public List<Party>? Parties { get; set; }

    /// <summary>
    /// Gets or sets the dialogs in the vCon.
    /// </summary>
    [JsonPropertyName("dialog")]
    public List<Dialog>? Dialog { get; set; }

    /// <summary>
    /// Gets or sets the attachments in the vCon.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment>? Attachments { get; set; }

    /// <summary>
    /// Gets or sets the analysis in the vCon.
    /// </summary>
    [JsonPropertyName("analysis")]
    public List<Analysis>? Analysis { get; set; }

    /// <summary>
    /// Gets or sets the tags in the vCon.
    /// </summary>
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets the signatures in the vCon.
    /// </summary>
    [JsonPropertyName("signatures")]
    public List<Signature>? Signatures { get; set; }

    private void SetDialogProperty(Dialog dialog, string key, object value)
    {
        switch (key)
        {
            case "originator":
                if (value is System.Text.Json.JsonElement jsonElement)
                {
                    dialog.Originator = jsonElement.GetInt32();
                }
                else
                {
                    dialog.Originator = Convert.ToInt32(value);
                }

                break;
            case "mimetype":
                dialog.Mimetype = value.ToString();
                break;
            case "filename":
                dialog.Filename = value.ToString();
                break;
            case "body":
                dialog.Body = value.ToString();
                break;
            case "encoding":
                dialog.Encoding = value.ToString();
                break;
            case "url":
                dialog.Url = value.ToString();
                break;
            case "duration":
                if (value is System.Text.Json.JsonElement elem)
                {
                    dialog.Duration = elem.GetInt32();
                }
                else
                {
                    dialog.Duration = Convert.ToInt32(value);
                }

                break;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VCon"/> class.
    /// Creates a new instance of VCon.
    /// </summary>
    /// <param name="logger">The logger for the VCon instance.</param>
    public VCon(ILogger<VCon>? logger = null)
    {
        _logger = logger ?? NullLoggerFactory.Instance.CreateLogger<VCon>();

        Uuid = Guid.NewGuid().ToString();
        Vcon = "1.0";  // Set the default vCon specification version (required by API)
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Parties = new List<Party>();
        Dialog = new List<Dialog>();
        Attachments = new List<Attachment>();
        Analysis = new List<Analysis>();
        Tags = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VCon"/> class.
    /// Parameterless constructor for JSON deserialization.
    /// </summary>
    [JsonConstructor]
    public VCon()
        : this(null)
    {
    }

    /// <summary>
    /// Creates a new vCon.
    /// </summary>
    /// <param name="logger">Optional logger for the new VCon instance.</param>
    /// <returns>A new VCon instance.</returns>
    public static VCon BuildNew(ILogger<VCon>? logger = null)
    {
        return new VCon(logger);
    }

    /// <summary>
    /// Creates a vCon from a JSON string.
    /// </summary>
    public static VCon BuildFromJson(string jsonString)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var vcon = JsonSerializer.Deserialize<VCon>(jsonString, options);

            if (vcon == null)
            {
                throw new JsonException("Failed to deserialize vCon");
            }

            return vcon;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to parse vCon JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets a tag value by name.
    /// </summary>
    public string? GetTag(string tagName)
    {
        if (Tags == null || !Tags.TryGetValue(tagName, out var value))
        {
            return null;
        }

        return value;
    }

    /// <summary>
    /// Adds a tag to the vCon.
    /// </summary>
    public void AddTag(string tagName, string tagValue)
    {
        Tags ??= new Dictionary<string, string>();
        Tags[tagName] = tagValue;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Finds an attachment by type.
    /// </summary>
    public Attachment? FindAttachmentByType(string type)
    {
        return Attachments?.FirstOrDefault(a => a.Type == type);
    }

    /// <summary>
    /// Adds an attachment to the vCon.
    /// </summary>
    public Attachment AddAttachment(string type, object body, Encoding encoding = Encoding.None)
    {
        var attachment = new Attachment(type, body, encoding);

        Attachments ??= new List<Attachment>();
        Attachments.Add(attachment);
        UpdatedAt = DateTime.UtcNow;

        return attachment;
    }

    /// <summary>
    /// Finds analysis by type.
    /// </summary>
    public Analysis? FindAnalysisByType(string type)
    {
        return Analysis?.FirstOrDefault(a => a.Type == type);
    }

    /// <summary>
    /// Adds analysis to the vCon.
    /// </summary>
    public void AddAnalysis(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("type", out var type) || type == null)
        {
            throw new ArgumentException("Type is required", nameof(parameters));
        }

        if (!parameters.TryGetValue("dialog", out var dialog) || dialog == null)
        {
            throw new ArgumentException("Dialog is required", nameof(parameters));
        }

        if (!parameters.TryGetValue("vendor", out var vendor) || vendor == null)
        {
            throw new ArgumentException("Vendor is required", nameof(parameters));
        }

        if (!parameters.TryGetValue("body", out var body) || body == null)
        {
            throw new ArgumentException("Body is required", nameof(parameters));
        }

        var analysis = new Analysis
        {
            Type = type.ToString()!,
            Dialog = dialog,
            Vendor = vendor.ToString()!,
            Body = body,
        };

        if (parameters.TryGetValue("encoding", out var encoding) && encoding != null)
        {
            analysis.Encoding = encoding.ToString()!.ToLowerInvariant() switch
            {
                "base64" => Encoding.Base64,
                "base64url" => Encoding.Base64Url,
                "json" => Encoding.Json,
                _ => Encoding.None,
            };
        }

        if (parameters.TryGetValue("extra", out var extra) && extra is Dictionary<string, object> extraDict)
        {
            analysis.Extra = extraDict;
        }

        Analysis ??= new List<Analysis>();
        Analysis.Add(analysis);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a party to the vCon.
    /// </summary>
    public void AddParty(Party party)
    {
        Parties ??= new List<Party>();
        Parties.Add(party);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a dialog to the vCon.
    /// </summary>
    public void AddDialog(Dialog dialog)
    {
        Dialog ??= new List<Dialog>();
        Dialog.Add(dialog);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Signs the vCon with a private key.
    /// </summary>
    public void Sign(string privateKeyPem)
    {
        try
        {
            _logger.LogInformation("Signing vCon with JWS");

            // Convert the vCon to a JSON string for signing
            var payload = ToJson();

            // Create a header for JWS
            var header = new
            {
                alg = "RS256",
                typ = "JWS",
            };

            // Create base64url encoded versions
            var headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
            var payloadBase64 = Base64UrlEncode(payload);

            // Create the signature input
            var signatureInput = $"{headerBase64}.{payloadBase64}";

            // Create signature using RSA with SHA-256
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);

            byte[] signature = rsa.SignData(
                System.Text.Encoding.UTF8.GetBytes(signatureInput),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            var signatureBase64 = Convert.ToBase64String(signature)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            // Create signature object
            Signatures ??= new List<Signature>();
            Signatures.Add(new Signature
            {
                Header = new Dictionary<string, string>
                {
                    ["alg"] = "RS256",
                    ["typ"] = "JWS",
                },
                JwsSignature = signatureBase64,
            });

            _logger.LogInformation("Successfully signed vCon with JWS");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to sign vCon: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifies the vCon signature with a public key.
    /// </summary>
    public bool Verify(string publicKeyPem)
    {
        try
        {
            if (Signatures == null || !Signatures.Any())
            {
                Console.WriteLine("No signatures found to verify");
                return false;
            }

            // Get the first signature
            var signature = Signatures[0];

            // Ensure we have a header with alg = RS256
            if (signature.Header == null || !signature.Header.TryGetValue("alg", out var alg) || alg != "RS256")
            {
                Console.WriteLine("Signature algorithm not supported");
                return false;
            }

            // Create a new vCon without the signatures for verification
            var verificationVCon = new VCon(_logger)
            {
                Uuid = Uuid,
                Vcon = Vcon,
                Subject = Subject,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                RedactedContent = RedactedContent,
                AppendedContent = AppendedContent,
                Group = Group,
                Meta = Meta,
                Parties = Parties,
                Dialog = Dialog,
                Attachments = Attachments,
                Analysis = Analysis,
                Tags = Tags,
            };

            // Convert the vCon to a JSON string for verification
            var payload = verificationVCon.ToJson();

            // Recreate the header for JWS
            var headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(signature.Header));
            var payloadBase64 = Base64UrlEncode(payload);

            // Create the signature input
            var signatureInput = $"{headerBase64}.{payloadBase64}";

            // Decode the signature
            var signatureBytes = Convert.FromBase64String(
                signature.JwsSignature
                    .Replace('-', '+')
                    .Replace('_', '/')
                + new string('=', (4 - (signature.JwsSignature.Length % 4)) % 4));

            // Verify signature using RSA with SHA-256
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            return rsa.VerifyData(
                System.Text.Encoding.UTF8.GetBytes(signatureInput),
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to verify vCon: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Generates a new RSA key pair for signing vCons.
    /// </summary>
    public static (string privateKey, string publicKey) GenerateKeyPair()
    {
        Console.WriteLine("Generating new RSA key pair");

        using var rsa = RSA.Create(2048);
        var privateKey = rsa.ExportRSAPrivateKeyPem();
        var publicKey = rsa.ExportRSAPublicKeyPem();

        Console.WriteLine("Successfully generated RSA key pair");
        return (privateKey, publicKey);
    }

    /// <summary>
    /// Converts the vCon to a dictionary representation.
    /// </summary>
    public Dictionary<string, object?> ToDict()
    {
        var dict = new Dictionary<string, object?>();

        if (Uuid != null)
        {
            dict["uuid"] = Uuid;
        }

        if (Vcon != null)
        {
            dict["vcon"] = Vcon;
        }

        if (Subject != null)
        {
            dict["subject"] = Subject;
        }

        if (CreatedAt != null)
        {
            dict["created_at"] = CreatedAt.Value.ToString("o");
        }

        if (UpdatedAt != null)
        {
            dict["updated_at"] = UpdatedAt.Value.ToString("o");
        }

        if (RedactedContent != null)
        {
            dict["redacted"] = RedactedContent;
        }

        if (AppendedContent != null)
        {
            dict["appended"] = AppendedContent;
        }

        if (Group != null)
        {
            dict["group"] = Group;
        }

        if (Meta != null)
        {
            dict["meta"] = Meta;
        }

        if (Parties != null)
        {
            dict["parties"] = Parties;
        }

        if (Dialog != null)
        {
            dict["dialog"] = Dialog;
        }

        if (Attachments != null)
        {
            dict["attachments"] = Attachments;
        }

        if (Analysis != null)
        {
            dict["analysis"] = Analysis;
        }

        if (Tags != null)
        {
            dict["tags"] = Tags;
        }

        if (Signatures != null)
        {
            dict["signatures"] = Signatures;
        }

        return dict;
    }

    /// <summary>
    /// Converts the vCon to a JSON string.
    /// </summary>
    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        return JsonSerializer.Serialize(this, options);
    }

    /// <summary>
    /// Encodes a string as Base64URL.
    /// </summary>
    private string Base64UrlEncode(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
