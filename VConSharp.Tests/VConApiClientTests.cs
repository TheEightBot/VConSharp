using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;

namespace VConSharp.Tests;

#pragma warning disable CA1001
public class VConApiClientTests
#pragma warning restore CA1001
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly ApiClient.VConApiClient _apiClient;

    public VConApiClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://example.com/api/"),
        };

        var options = new ApiClient.VConApiClientOptions
        {
            BaseUrl = "https://example.com/api/",
            ApiToken = "test-token",
        };

        _apiClient = new ApiClient.VConApiClient(_httpClient, options, NullLogger<ApiClient.VConApiClient>.Instance);
    }

    [Fact]
    public async Task GetVConsUuidsAsync_ReturnsListOfUuids()
    {
        // Arrange
        var expectedUuids = new List<string>
        {
            "550e8400-e29b-41d4-a716-446655440000",
            "550e8400-e29b-41d4-a716-446655440001",
        };

        var jsonResponse = JsonSerializer.Serialize(expectedUuids);

        SetupMockResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _apiClient.GetVConsUuidsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUuids.Count, result.Count);
        Assert.Equal(expectedUuids[0], result[0]);
        Assert.Equal(expectedUuids[1], result[1]);

        VerifyHttpRequest(HttpMethod.Get, "vcon");
    }

    [Fact]
    public async Task GetVConAsync_ReturnsVCon()
    {
        // Arrange
        var uuid = Guid.NewGuid();
        var vconData = new
        {
            vcon = "1.0.0",
            uuid = uuid.ToString(),
            created_at = DateTime.UtcNow.ToString("o"),
            subject = "Test vCon",
            parties = new List<object>(),
            dialog = new List<object>(),
            analysis = new List<object>(),
            attachments = new List<object>(),
        };

        var jsonResponse = JsonSerializer.Serialize(vconData);

        SetupMockResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _apiClient.GetVConAsync(uuid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uuid.ToString(), result.Uuid);
        Assert.Equal("Test vCon", result.Subject);

        VerifyHttpRequest(HttpMethod.Get, $"vcon/{uuid}");
    }

    [Fact]
    public async Task CreateVConAsync_CreatesAndReturnsVCon()
    {
        // Arrange
        var vcon = new VCon();
        var party = new Party { Name = "Test User", Tel = "+1234567890", };
        vcon.AddParty(party);

        var uuid = Guid.NewGuid();
        var vconData = new
        {
            vcon = "1.0.0",
            uuid = uuid.ToString(),
            created_at = DateTime.UtcNow.ToString("o"),
            subject = "Test vCon",
            parties = new List<object>
            {
                new { name = "Test User", tel = "+1234567890", },
            },
            dialog = new List<object>(),
            analysis = new List<object>(),
            attachments = new List<object>(),
        };

        var jsonResponse = JsonSerializer.Serialize(vconData);

        SetupMockResponse(HttpStatusCode.Created, jsonResponse);

        // Act
        var result = await _apiClient.CreateVConAsync(vcon);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uuid.ToString(), result.Uuid);
        Assert.Equal("Test vCon", result.Subject);
        Assert.Single(result.Parties);

        VerifyHttpRequest(HttpMethod.Post, "vcon");
    }

    [Fact]
    public async Task DeleteVConAsync_DeletesVCon()
    {
        // Arrange
        var uuid = Guid.NewGuid();

        SetupMockResponse(HttpStatusCode.NoContent, string.Empty);

        // Act
        await _apiClient.DeleteVConAsync(uuid);

        // Assert - no exception means success
        VerifyHttpRequest(HttpMethod.Delete, $"vcon/{uuid}");
    }

    [Fact]
    public async Task SearchVConsAsync_ReturnsMatchingUuids()
    {
        // Arrange
        var expectedUuids = new List<string>
        {
            "550e8400-e29b-41d4-a716-446655440000",
            "550e8400-e29b-41d4-a716-446655440001",
        };

        var jsonResponse = JsonSerializer.Serialize(expectedUuids);

        SetupMockResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _apiClient.SearchVConsAsync(tel: "+1234567890");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        VerifyHttpRequest(HttpMethod.Get, "vcons/search?tel=%2B1234567890");
    }

    private void SetupMockResponse(HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json"),
            });
    }

    private void VerifyHttpRequest(HttpMethod method, string requestUri)
    {
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri!.PathAndQuery.Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>());
    }
}
