namespace VConSharp.Tests;

public class DialogTests
{
    [Fact]
    public void ShouldCreateDialogWithMinimalProperties()
    {
        // Arrange
        var start = DateTime.UtcNow;

        // Act
        var dialog = new Dialog("text/plain", start, new[] { 0, 1, });

        // Assert
        Assert.Equal("text/plain", dialog.Type);
        Assert.Equal(start, dialog.Start);
        Assert.Equal(new List<int> { 0, 1, }, dialog.Parties);

        var dict = dialog.ToDict();
        Assert.Equal("text/plain", dict["type"]);
        Assert.Equal(start, dict["start"]);
        Assert.Equal(new List<int> { 0, 1, }, dict["parties"]);
    }

    [Fact]
    public void ShouldCreateDialogWithAllProperties()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var partyHistory = new PartyHistory(0, "joined", start);

        // Act
        var dialog = new Dialog("text/plain", start, new[] { 0, 1, })
        {
            Originator = 0,
            Mimetype = "text/plain",
            Filename = "conversation.txt",
            Body = "Hello!",
            Encoding = "utf-8",
            Url = "https://example.com/audio.wav",
            Alg = "sha256",
            Signature = "signature",
            Disposition = "inline",
            PartyHistory = new List<PartyHistory> { partyHistory, },
            Transferee = 1,
            Transferor = 0,
            TransferTarget = 2,
            Original = 0,
            Consultation = 1,
            TargetDialog = 0,
            Campaign = "support",
            Interaction = "call",
            Skill = "sales",
            Duration = 300,
            Meta = new Dictionary<string, object> { ["key"] = "value", },
        };

        // Assert
        Assert.Equal("text/plain", dialog.Type);
        Assert.Equal(start, dialog.Start);
        Assert.Equal(new List<int> { 0, 1, }, dialog.Parties);
        Assert.Equal(0, dialog.Originator);
        Assert.Equal("text/plain", dialog.Mimetype);
        Assert.Equal("conversation.txt", dialog.Filename);
        Assert.Equal("Hello!", dialog.Body);
        Assert.Equal("utf-8", dialog.Encoding);
        Assert.Equal("https://example.com/audio.wav", dialog.Url);
        Assert.Equal("sha256", dialog.Alg);
        Assert.Equal("signature", dialog.Signature);
        Assert.Equal("inline", dialog.Disposition);
        Assert.Single(dialog.PartyHistory);
        Assert.Equal(1, dialog.Transferee);
        Assert.Equal(0, dialog.Transferor);
        Assert.Equal(2, dialog.TransferTarget);
        Assert.Equal(0, dialog.Original);
        Assert.Equal(1, dialog.Consultation);
        Assert.Equal(0, dialog.TargetDialog);
        Assert.Equal("support", dialog.Campaign);
        Assert.Equal("call", dialog.Interaction);
        Assert.Equal("sales", dialog.Skill);
        Assert.Equal(300, dialog.Duration);
        Assert.Equal("value", dialog.Meta["key"]);
    }

    [Fact]
    public void ShouldHandleExternalData()
    {
        // Arrange
        var dialog = new Dialog("text/plain", DateTime.UtcNow, new[] { 0, });

        // Act
        dialog.AddExternalData(
            "https://example.com/audio.wav",
            "audio.wav",
            "audio/wav");

        // Assert
        Assert.True(dialog.IsExternalData());
        Assert.False(dialog.IsInlineData());
        Assert.Equal("https://example.com/audio.wav", dialog.Url);
        Assert.Equal("audio.wav", dialog.Filename);
        Assert.Equal("audio/wav", dialog.Mimetype);
        Assert.Null(dialog.Body);
        Assert.Null(dialog.Encoding);
    }

    [Fact]
    public void ShouldHandleInlineData()
    {
        // Arrange
        var dialog = new Dialog("text/plain", DateTime.UtcNow, new[] { 0, });

        // Act
        dialog.AddInlineData(
            "Hello!",
            "message.txt",
            "text/plain");

        // Assert
        Assert.False(dialog.IsExternalData());
        Assert.True(dialog.IsInlineData());
        Assert.Equal("Hello!", dialog.Body);
        Assert.Equal("message.txt", dialog.Filename);
        Assert.Equal("text/plain", dialog.Mimetype);
        Assert.Null(dialog.Url);
    }

    [Fact]
    public void ShouldValidateMimeTypes()
    {
        // Arrange
        var dialog = new Dialog("text/plain", DateTime.UtcNow, new[] { 0, });

        // Act & Assert
        Assert.Throws<ArgumentException>(() => dialog.AddExternalData(
            "https://example.com/file",
            "file",
            "invalid/mime"));

        Assert.Throws<ArgumentException>(() => dialog.AddInlineData(
            "content",
            "file",
            "invalid/mime"));
    }

    [Fact]
    public void ShouldCheckContentTypes()
    {
        // Arrange
        var dialog = new Dialog("text/plain", DateTime.UtcNow, new[] { 0, });
        dialog.Mimetype = "text/plain";

        // Act & Assert
        Assert.True(dialog.IsText());
        Assert.False(dialog.IsAudio());
        Assert.False(dialog.IsVideo());
        Assert.False(dialog.IsEmail());

        dialog.Mimetype = "audio/wav";
        Assert.False(dialog.IsText());
        Assert.True(dialog.IsAudio());

        dialog.Mimetype = "video/mp4";
        Assert.True(dialog.IsVideo());

        dialog.Mimetype = "message/rfc822";
        Assert.True(dialog.IsEmail());
    }
}
