using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace VConSharp
{
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
        /// Gets or sets the data contained in the vCon.
        /// </summary>
        private VConData Data { get; set; }

        /// <summary>
        /// Gets the UUID of the vCon.
        /// </summary>
        public string Uuid => Data.Uuid ?? string.Empty;

        /// <summary>
        /// Gets the vCon specification version.
        /// </summary>
        public string VconVersion => Data.Vcon ?? string.Empty;

        /// <summary>
        /// Gets the subject of the vCon.
        /// </summary>
        public string? Subject => Data.Subject;

        /// <summary>
        /// Gets the creation time of the vCon.
        /// </summary>
        public DateTime CreatedAt => Data.CreatedAt ?? DateTime.UtcNow;

        /// <summary>
        /// Gets the last update time of the vCon.
        /// </summary>
        public DateTime UpdatedAt => Data.UpdatedAt ?? DateTime.UtcNow;

        /// <summary>
        /// Gets a value indicating whether gets whether the vCon has been redacted.
        /// </summary>
        public bool Redacted => Data.Redacted ?? false;

        /// <summary>
        /// Gets a value indicating whether gets whether the vCon has been appended.
        /// </summary>
        public bool Appended => Data.Appended ?? false;

        /// <summary>
        /// Gets the group of the vCon.
        /// </summary>
        public string? Group => Data.Group;

        /// <summary>
        /// Gets the metadata of the vCon.
        /// </summary>
        public Dictionary<string, object>? Meta => Data.Meta;

        /// <summary>
        /// Gets the parties in the vCon.
        /// </summary>
        public List<Party> Parties => Data.Parties?.Select(p => new Party(p)).ToList() ?? new List<Party>();

        /// <summary>
        /// Gets the dialogs in the vCon.
        /// </summary>
        public List<Dialog> Dialogs
        {
            get
            {
                if (Data.Dialog == null)
                {
                    return new List<Dialog>();
                }

                var result = new List<Dialog>();
                foreach (var d in Data.Dialog)
                {
                    // Handle deserialized JSON data
                    if (d.TryGetValue("type", out var type) &&
                        d.TryGetValue("start", out var startObj) &&
                        d.TryGetValue("parties", out var partiesObj))
                    {
                        try
                        {
                            var start = startObj is DateTime dt ? dt : DateTime.Parse(startObj.ToString() ?? string.Empty);
                            var parties = new List<int>();

                            // Handle array of parties
                            if (partiesObj is System.Text.Json.JsonElement jsonElement && jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                foreach (var element in jsonElement.EnumerateArray())
                                {
                                    parties.Add(element.GetInt32());
                                }
                            }
                            else if (partiesObj is IEnumerable<object> objList)
                            {
                                foreach (var obj in objList)
                                {
                                    if (obj is System.Text.Json.JsonElement elem)
                                    {
                                        parties.Add(elem.GetInt32());
                                    }
                                    else
                                    {
                                        parties.Add(Convert.ToInt32(obj));
                                    }
                                }
                            }
                            else if (partiesObj is IEnumerable<int> intList)
                            {
                                parties.AddRange(intList);
                            }

                            var dialog = new Dialog(type.ToString() ?? string.Empty, start, parties);

                            // Set other properties
                            foreach (var prop in d)
                            {
                                if (prop.Key != "type" && prop.Key != "start" && prop.Key != "parties")
                                {
                                    SetDialogProperty(dialog, prop.Key, prop.Value);
                                }
                            }

                            result.Add(dialog);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing dialog");
                        }
                    }
                }

                return result;
            }
        }

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
        /// Gets the attachments in the vCon.
        /// </summary>
        public List<Attachment> Attachments => Data.Attachments ?? new List<Attachment>();

        /// <summary>
        /// Gets the analysis in the vCon.
        /// </summary>
        public List<Analysis> Analysis => Data.Analysis ?? new List<Analysis>();

        /// <summary>
        /// Gets the tags in the vCon.
        /// </summary>
        public Dictionary<string, string>? Tags => Data.Tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCon"/> class.
        /// Creates a new instance of VCon.
        /// </summary>
        /// <param name="logger">The logger for the VCon instance.</param>
        public VCon(ILogger<VCon>? logger = null)
        {
            _logger = logger ?? NullLoggerFactory.Instance.CreateLogger<VCon>();

            Data = new VConData
            {
                Uuid = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Parties = new List<Dictionary<string, object>>(),
                Dialog = new List<Dictionary<string, object>>(),
                Attachments = new List<Attachment>(),
                Analysis = new List<Analysis>(),
                Tags = new Dictionary<string, string>(),
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VCon"/> class.
        /// Creates a new instance of VCon with the specified data.
        /// </summary>
        /// <param name="data">The VCon data.</param>
        /// <param name="logger">The logger for the VCon instance.</param>
        public VCon(VConData data, ILogger<VCon>? logger = null)
        {
            _logger = logger ?? NullLoggerFactory.Instance.CreateLogger<VCon>();
            Data = data;
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

                var data = JsonSerializer.Deserialize<VConData>(jsonString, options);

                if (data == null)
                {
                    throw new JsonException("Failed to deserialize vCon data");
                }

                return new VCon(data, null);
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
            if (Data.Tags == null || !Data.Tags.TryGetValue(tagName, out var value))
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
            Data.Tags ??= new Dictionary<string, string>();
            Data.Tags[tagName] = tagValue;
            Data.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Finds an attachment by type.
        /// </summary>
        public Attachment? FindAttachmentByType(string type)
        {
            return Data.Attachments?.FirstOrDefault(a => a.Type == type);
        }

        /// <summary>
        /// Adds an attachment to the vCon.
        /// </summary>
        public Attachment AddAttachment(string type, object body, Encoding encoding = Encoding.None)
        {
            var attachment = new Attachment(type, body, encoding);

            Data.Attachments ??= new List<Attachment>();
            Data.Attachments.Add(attachment);
            Data.UpdatedAt = DateTime.UtcNow;

            return attachment;
        }

        /// <summary>
        /// Finds analysis by type.
        /// </summary>
        public Analysis? FindAnalysisByType(string type)
        {
            return Data.Analysis?.FirstOrDefault(a => a.Type == type);
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

            Data.Analysis ??= new List<Analysis>();
            Data.Analysis.Add(analysis);
            Data.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a party to the vCon.
        /// </summary>
        public void AddParty(Party party)
        {
            Data.Parties ??= new List<Dictionary<string, object>>();
            Data.Parties.Add(party.ToDict() as Dictionary<string, object>);
            Data.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Finds a party index by a property value.
        /// </summary>
        public int? FindPartyIndex(string by, string val)
        {
            if (Data.Parties == null)
            {
                return null;
            }

            for (int i = 0; i < Data.Parties.Count; i++)
            {
                if (Data.Parties[i].TryGetValue(by, out var value) && value.ToString() == val)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a dialog by a property value.
        /// </summary>
        public Dialog? FindDialog(string by, object val)
        {
            if (Data.Dialog == null)
            {
                return null;
            }

            var dialog = Data.Dialog.FirstOrDefault(d => d.TryGetValue(by, out var value) && value.Equals(val));
            return dialog != null ? new Dialog(dialog) : null;
        }

        /// <summary>
        /// Adds a dialog to the vCon.
        /// </summary>
        public void AddDialog(Dialog dialog)
        {
            Data.Dialog ??= new List<Dictionary<string, object>>();
            Data.Dialog.Add(dialog.ToDict() as Dictionary<string, object>);
            Data.UpdatedAt = DateTime.UtcNow;
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
                Data.Signatures ??= new List<Signature>();
                Data.Signatures.Add(new Signature
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
                if (Data.Signatures == null || !Data.Signatures.Any())
                {
                    Console.WriteLine("No signatures found to verify");
                    return false;
                }

                // Get the first signature
                var signature = Data.Signatures[0];

                // Ensure we have a header with alg = RS256
                if (signature.Header == null || !signature.Header.TryGetValue("alg", out var alg) || alg != "RS256")
                {
                    Console.WriteLine("Signature algorithm not supported");
                    return false;
                }

                // Create a new vCon without the signatures for verification
                var verificationVCon = new VCon(_logger)
                {
                    Data = new VConData
                    {
                        Uuid = Data.Uuid,
                        Vcon = Data.Vcon,
                        Subject = Data.Subject,
                        CreatedAt = Data.CreatedAt,
                        UpdatedAt = Data.UpdatedAt,
                        Redacted = Data.Redacted,
                        Appended = Data.Appended,
                        Group = Data.Group,
                        Meta = Data.Meta,
                        Parties = Data.Parties,
                        Dialog = Data.Dialog,
                        Attachments = Data.Attachments,
                        Analysis = Data.Analysis,
                        Tags = Data.Tags,
                    },
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

            if (Data.Uuid != null)
            {
                dict["uuid"] = Data.Uuid;
            }

            if (Data.Vcon != null)
            {
                dict["vcon"] = Data.Vcon;
            }

            if (Data.Subject != null)
            {
                dict["subject"] = Data.Subject;
            }

            if (Data.CreatedAt != null)
            {
                dict["created_at"] = Data.CreatedAt.Value.ToString("o");
            }

            if (Data.UpdatedAt != null)
            {
                dict["updated_at"] = Data.UpdatedAt.Value.ToString("o");
            }

            if (Data.Redacted != null)
            {
                dict["redacted"] = Data.Redacted;
            }

            if (Data.Appended != null)
            {
                dict["appended"] = Data.Appended;
            }

            if (Data.Group != null)
            {
                dict["group"] = Data.Group;
            }

            if (Data.Meta != null)
            {
                dict["meta"] = Data.Meta;
            }

            if (Data.Parties != null)
            {
                dict["parties"] = Data.Parties;
            }

            if (Data.Dialog != null)
            {
                dict["dialog"] = Data.Dialog;
            }

            if (Data.Attachments != null)
            {
                dict["attachments"] = Data.Attachments;
            }

            if (Data.Analysis != null)
            {
                dict["analysis"] = Data.Analysis;
            }

            if (Data.Tags != null)
            {
                dict["tags"] = Data.Tags;
            }

            if (Data.Signatures != null)
            {
                dict["signatures"] = Data.Signatures;
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

            return JsonSerializer.Serialize(Data, options);
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

    /// <summary>
    /// Represents the data structure of a vCon.
    /// </summary>
    public class VConData
    {
        /// <summary>
        /// Gets or sets the UUID of the vCon.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        /// <summary>
        /// Gets or sets the vCon specification version.
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
        /// Gets or sets whether the vCon has been redacted.
        /// </summary>
        [JsonPropertyName("redacted")]
        public bool? Redacted { get; set; }

        /// <summary>
        /// Gets or sets whether the vCon has been appended.
        /// </summary>
        [JsonPropertyName("appended")]
        public bool? Appended { get; set; }

        /// <summary>
        /// Gets or sets the group of the vCon.
        /// </summary>
        [JsonPropertyName("group")]
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets additional metadata for the vCon.
        /// </summary>
        [JsonPropertyName("meta")]
        public Dictionary<string, object>? Meta { get; set; }

        /// <summary>
        /// Gets or sets the parties in the vCon.
        /// </summary>
        [JsonPropertyName("parties")]
        public List<Dictionary<string, object>>? Parties { get; set; }

        /// <summary>
        /// Gets or sets the dialogs in the vCon.
        /// </summary>
        [JsonPropertyName("dialog")]
        public List<Dictionary<string, object>>? Dialog { get; set; }

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
    }
}
