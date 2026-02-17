using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace VConSharp.ApiClient;

/// <summary>
/// Client for interacting with the vCon API.
/// </summary>
public class VConApiClient : IVConApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VConApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VConApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="options">The API client options.</param>
    /// <param name="logger">The logger.</param>
    public VConApiClient(
        HttpClient httpClient,
        VConApiClientOptions options,
        ILogger<VConApiClient>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<VConApiClient>.Instance;

        // Configure the HTTP client
        _httpClient.BaseAddress = new Uri(options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(options.ApiToken))
        {
            _httpClient.DefaultRequestHeaders.Add("x-conserver-api-token", options.ApiToken);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetVConsUuidsAsync(
        int? page = 1,
        int? size = 50,
        DateTime? since = null,
        DateTime? until = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (page.HasValue)
        {
            queryParams.Add($"page={page.Value}");
        }

        if (size.HasValue)
        {
            queryParams.Add($"size={size.Value}");
        }

        if (since.HasValue)
        {
            queryParams.Add($"since={since.Value:o}");
        }

        if (until.HasValue)
        {
            queryParams.Add($"until={until.Value:o}");
        }

        var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        var requestUri = $"vcon{queryString}";

        try
        {
            _logger.LogDebug("Getting vCons UUIDs with parameters: {Parameters}", queryParams);
            var response = await _httpClient.GetFromJsonAsync<List<string>>(requestUri, _jsonOptions, cancellationToken).ConfigureAwait(false);
            return response ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vCon UUIDs");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<VCon?> GetVConAsync(Guid vconUuid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting vCon with UUID: {UUID}", vconUuid);
            var requestUri = $"vcon/{vconUuid}";
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            // Read the raw JSON first to inspect it
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var vconData = JsonSerializer.Deserialize<VCon>(stream, _jsonOptions);
                return vconData;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize VCon JSON. Error at path: {Path}", jsonEx.Path);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vCon with UUID: {UUID}", vconUuid);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<VCon>> GetMultipleVConsAsync(
        IEnumerable<Guid> vconUuids,
        CancellationToken cancellationToken = default)
    {
        var uuidList = vconUuids.ToList();
        if (!uuidList.Any())
        {
            return new List<VCon>();
        }

        var queryParams = string.Join("&", uuidList.Select(uuid => $"vcon_uuids={uuid}"));
        var requestUri = $"vcons?{queryParams}";

        try
        {
            _logger.LogDebug("Getting multiple vCons with UUIDs: {UUIDs}", string.Join(", ", uuidList));
            var response = await _httpClient.GetFromJsonAsync<List<VCon>>(requestUri, _jsonOptions, cancellationToken).ConfigureAwait(false);

            return response?.Where(v => v != null).ToList() ?? new List<VCon>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple vCons");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<VCon> CreateVConAsync(VCon vcon, IEnumerable<string>? ingressLists = null, CancellationToken cancellationToken = default)
    {
        var requestUri = "vcon";

        if (ingressLists != null && ingressLists.Any())
        {
            var ingressListsParam = string.Join("&", ingressLists.Select(list => $"ingress_lists={list}"));
            requestUri = $"{requestUri}?{ingressListsParam}";
        }

        try
        {
            _logger.LogDebug("Creating vCon with UUID: {UUID}", vcon.Uuid);

            using var response = await _httpClient.PostAsJsonAsync(requestUri, vcon, _jsonOptions, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("API returned error {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
            }

            response.EnsureSuccessStatusCode();

            var createdVCon = await response.Content.ReadFromJsonAsync<VCon>(_jsonOptions, cancellationToken).ConfigureAwait(false);
            return createdVCon!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vCon");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteVConAsync(Guid vconUuid, CancellationToken cancellationToken = default)
    {
        var requestUri = $"vcon/{vconUuid}";

        try
        {
            _logger.LogDebug("Deleting vCon with UUID: {UUID}", vconUuid);
            using var response = await _httpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vCon with UUID: {UUID}", vconUuid);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Guid>> SearchVConsAsync(
        string? tel = null,
        string? mailto = null,
        string? name = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(tel))
        {
            queryParams.Add($"tel={Uri.EscapeDataString(tel)}");
        }

        if (!string.IsNullOrEmpty(mailto))
        {
            queryParams.Add($"mailto={Uri.EscapeDataString(mailto)}");
        }

        if (!string.IsNullOrEmpty(name))
        {
            queryParams.Add($"name={Uri.EscapeDataString(name)}");
        }

        var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        var requestUri = $"vcons/search{queryString}";

        try
        {
            _logger.LogDebug("Searching vCons with parameters: {Parameters}", queryParams);
            var response = await _httpClient.GetFromJsonAsync<List<string>>(requestUri, _jsonOptions, cancellationToken).ConfigureAwait(false);

            return response?.Select(Guid.Parse).ToList() ?? new List<Guid>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for vCons");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task AddToIngressListAsync(string ingressList, IEnumerable<Guid> vconUuids, CancellationToken cancellationToken = default)
    {
        var uuids = vconUuids.Select(u => u.ToString()).ToList();
        var requestUri = $"vcon/ingress?ingress_list={Uri.EscapeDataString(ingressList)}";

        try
        {
            _logger.LogDebug("Adding vCons to ingress list {IngressList}: {UUIDs}", ingressList, string.Join(", ", uuids));
            using var response = await _httpClient.PostAsJsonAsync(requestUri, uuids, _jsonOptions, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vCons to ingress list");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetFromEgressListAsync(string egressList, int? limit = 1, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string> { $"egress_list={Uri.EscapeDataString(egressList)}", };

        if (limit.HasValue)
        {
            queryParams.Add($"limit={limit.Value}");
        }

        var queryString = string.Join("&", queryParams);
        var requestUri = $"vcon/egress?{queryString}";

        try
        {
            _logger.LogDebug("Getting vCons from egress list {EgressList} with limit {Limit}", egressList, limit);
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new List<string>();
            }

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var results = JsonSerializer.Deserialize<List<string>>(stream, _jsonOptions);
            return results ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vCons from egress list");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> CountEgressListAsync(string egressList, CancellationToken cancellationToken = default)
    {
        var requestUri = $"vcon/count?egress_list={Uri.EscapeDataString(egressList)}";

        try
        {
            _logger.LogDebug("Counting vCons in egress list {EgressList}", egressList);
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(content, _jsonOptions);

            return result?.FirstOrDefault().Value ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting vCons in egress list");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting system configuration");
            using var response = await _httpClient.GetAsync("config", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content, _jsonOptions);
            return result ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system configuration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateConfigAsync(Dictionary<string, object> config, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating system configuration");
            using var response = await _httpClient.PostAsJsonAsync("config", config, _jsonOptions, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating system configuration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> GetDeadLetterQueueAsync(string ingressList, CancellationToken cancellationToken = default)
    {
        var requestUri = $"dlq?ingress_list={Uri.EscapeDataString(ingressList)}";

        try
        {
            _logger.LogDebug("Getting DLQ contents for ingress list {IngressList}", ingressList);
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content, _jsonOptions);
            return result ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DLQ contents");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> ReprocessDeadLetterQueueAsync(string ingressList, CancellationToken cancellationToken = default)
    {
        var requestUri = $"dlq/reprocess?ingress_list={Uri.EscapeDataString(ingressList)}";

        try
        {
            _logger.LogDebug("Reprocessing DLQ for ingress list {IngressList}", ingressList);
            using var response = await _httpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content, _jsonOptions);
            return result ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reprocessing DLQ");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> RebuildSearchIndexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Rebuilding search index");
            using var response = await _httpClient.GetAsync("index_vcons", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content, _jsonOptions);
            return result ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding search index");
            throw;
        }
    }
}
