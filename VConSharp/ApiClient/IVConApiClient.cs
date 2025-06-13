namespace VConSharp.ApiClient;

/// <summary>
/// Interface for interacting with the vCon API.
/// </summary>
public interface IVConApiClient
{
    /// <summary>
    /// Gets a list of vCon UUIDs.
    /// </summary>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="since">Filter vCons created after this date.</param>
    /// <param name="until">Filter vCons created before this date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of vCon UUIDs.</returns>
    Task<IReadOnlyList<string>> GetVConsUuidsAsync(
        int? page = 1,
        int? size = 50,
        DateTime? since = null,
        DateTime? until = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a vCon by UUID.
    /// </summary>
    /// <param name="vconUuid">The UUID of the vCon.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The vCon if found; otherwise, null.</returns>
    Task<VCon?> GetVConAsync(Guid vconUuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple vCons by their UUIDs.
    /// </summary>
    /// <param name="vconUuids">The UUIDs of the vCons to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of vCons.</returns>
    Task<IReadOnlyList<VCon>> GetMultipleVConsAsync(
        IEnumerable<Guid> vconUuids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new vCon.
    /// </summary>
    /// <param name="vcon">The vCon to create.</param>
    /// <param name="ingressLists">Optional list of ingress queues to add the vCon to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created vCon.</returns>
    Task<VCon> CreateVConAsync(
        VCon vcon,
        IEnumerable<string>? ingressLists = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a vCon.
    /// </summary>
    /// <param name="vconUuid">The UUID of the vCon to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteVConAsync(Guid vconUuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for vCons using personal identifiers and metadata.
    /// </summary>
    /// <param name="tel">Phone number to search for.</param>
    /// <param name="mailto">Email address to search for.</param>
    /// <param name="name">Name of the party to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of vCon UUIDs matching the search criteria.</returns>
    Task<IReadOnlyList<Guid>> SearchVConsAsync(
        string? tel = null,
        string? mailto = null,
        string? name = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds vCon UUIDs to a processing chain.
    /// </summary>
    /// <param name="ingressList">Name of the ingress list to add to.</param>
    /// <param name="vconUuids">The UUIDs of the vCons to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddToIngressListAsync(
        string ingressList,
        IEnumerable<Guid> vconUuids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes vCon UUIDs from a chain output (egress).
    /// </summary>
    /// <param name="egressList">Name of the egress list to pop from.</param>
    /// <param name="limit">Maximum number of UUIDs to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of vCon UUIDs that were removed.</returns>
    Task<IReadOnlyList<string>> GetFromEgressListAsync(
        string egressList,
        int? limit = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of vCons in a chain's output.
    /// </summary>
    /// <param name="egressList">Name of the egress list to count.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of vCons in the egress list.</returns>
    Task<int> CountEgressListAsync(string egressList, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current system configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The system configuration.</returns>
    Task<Dictionary<string, object>> GetConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the system configuration.
    /// </summary>
    /// <param name="config">The new system configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateConfigAsync(Dictionary<string, object> config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of vCons in the dead letter queue.
    /// </summary>
    /// <param name="ingressList">Name of the ingress list to get DLQ for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The contents of the dead letter queue.</returns>
    Task<Dictionary<string, object>> GetDeadLetterQueueAsync(string ingressList, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves items from the dead letter queue back to ingress.
    /// </summary>
    /// <param name="ingressList">Name of the ingress list to reprocess to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the reprocessing operation.</returns>
    Task<Dictionary<string, object>> ReprocessDeadLetterQueueAsync(string ingressList, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rebuilds the vCon search index.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the rebuild operation.</returns>
    Task<Dictionary<string, object>> RebuildSearchIndexAsync(CancellationToken cancellationToken = default);
}
