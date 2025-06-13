namespace VConSharp.Tests;

public class VConTests
{
    [Fact]
    public void ShouldCreateVConWithDefaultValues()
    {
        // Arrange & Act
        var vcon = VCon.BuildNew();

        // Assert
        Assert.NotNull(vcon.Uuid);
        Assert.IsType<DateTime>(vcon.CreatedAt);
        Assert.IsType<DateTime>(vcon.UpdatedAt);
        Assert.Empty(vcon.Parties);
        Assert.Empty(vcon.Dialog);
        Assert.Empty(vcon.Attachments);
        Assert.Empty(vcon.Analysis);
        Assert.NotNull(vcon.Tags);
        Assert.Empty(vcon.Tags);
    }

    [Fact]
    public void ShouldCreateVConFromJson()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;

        var json = @$"{{
            ""uuid"": ""{uuid}"",
            ""created_at"": ""{createdAt:o}"",
            ""updated_at"": ""{updatedAt:o}"",
            ""parties"": [],
            ""dialog"": [],
            ""attachments"": [],
            ""analysis"": [],
            ""tags"": {{}}
        }}";

        // Act
        var vcon = VCon.BuildFromJson(json);

        // Assert
        Assert.Equal(uuid, vcon.Uuid);
        Assert.Empty(vcon.Parties);
        Assert.Empty(vcon.Dialog);
    }

    [Fact]
    public void ShouldAddAndFindParties()
    {
        // Arrange
        var vcon = VCon.BuildNew();
        var party = new Party(new Dictionary<string, object>
        {
            ["tel"] = "+1234567890",
            ["name"] = "John Doe",
        });

        // Act
        vcon.AddParty(party);

        // Assert
        Assert.Single(vcon.Parties);
        Assert.Equal("+1234567890", vcon.Parties[0].Tel);
        Assert.Equal("John Doe", vcon.Parties[0].Name);
    }

    [Fact]
    public void ShouldAddAndFindDialogs()
    {
        // Arrange
        var vcon = VCon.BuildNew();
        var dialog = new Dialog(
            "text/plain",
            DateTime.UtcNow,
            new[] { 0, 1, });

        dialog.Body = "Hello!";

        // Act
        vcon.AddDialog(dialog);

        // Assert
        Assert.Single(vcon.Dialog);
        Assert.Equal("Hello!", vcon.Dialog[0].Body);
    }

    [Fact]
    public void ShouldAddAndFindAttachments()
    {
        // Arrange
        var vcon = VCon.BuildNew();

        // Act
        var attachment = vcon.AddAttachment(
            "application/pdf",
            "base64EncodedContent",
            Encoding.Base64);

        // Assert
        Assert.Single(vcon.Attachments);
        Assert.Equal("application/pdf", vcon.Attachments[0].Type);

        var foundAttachment = vcon.FindAttachmentByType("application/pdf");
        Assert.NotNull(foundAttachment);
        Assert.Equal("application/pdf", foundAttachment.Type);
    }

    [Fact]
    public void ShouldAddAndFindAnalysis()
    {
        // Arrange
        var vcon = VCon.BuildNew();

        // Act
        vcon.AddAnalysis(new Dictionary<string, object>
        {
            ["type"] = "sentiment",
            ["dialog"] = 0,
            ["vendor"] = "sentiment-analyzer",
            ["body"] = new Dictionary<string, object>
            {
                ["score"] = 0.8,
                ["label"] = "positive",
            },
        });

        // Assert
        Assert.Single(vcon.Analysis);
        Assert.Equal("sentiment", vcon.Analysis[0].Type);

        var foundAnalysis = vcon.FindAnalysisByType("sentiment");
        Assert.NotNull(foundAnalysis);
        Assert.Equal("sentiment", foundAnalysis.Type);
    }

    [Fact]
    public void ShouldAddAndGetTags()
    {
        // Arrange
        var vcon = VCon.BuildNew();

        // Act
        vcon.AddTag("category", "support");

        // Assert
        Assert.Equal(new Dictionary<string, string> { ["category"] = "support", }, vcon.Tags);
        Assert.Equal("support", vcon.GetTag("category"));
    }

    [Fact]
    public void ShouldConvertToJsonAndBack()
    {
        // Arrange
        var vcon = VCon.BuildNew();
        var party = new Party(new Dictionary<string, object>
        {
            ["tel"] = "+1234567890",
            ["name"] = "John Doe",
        });

        vcon.AddParty(party);

        // Act
        var json = vcon.ToJson();
        var deserializedVcon = VCon.BuildFromJson(json);

        // Assert
        Assert.Equal(vcon.Uuid, deserializedVcon.Uuid);
        Assert.Single(deserializedVcon.Parties);
        Assert.Equal("+1234567890", deserializedVcon.Parties[0].Tel);
        Assert.Equal("John Doe", deserializedVcon.Parties[0].Name);
    }
}
