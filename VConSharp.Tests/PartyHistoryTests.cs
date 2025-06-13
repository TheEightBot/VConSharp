namespace VConSharp.Tests;

public class PartyHistoryTests
{
    [Fact]
    public void ShouldCreatePartyHistoryWithAllProperties()
    {
        // Arrange
        var time = DateTime.UtcNow;

        // Act
        var history = new PartyHistory(0, "joined", time);

        // Assert
        Assert.Equal(0, history.Party);
        Assert.Equal("joined", history.Event);
        Assert.Equal(time, history.Time);

        var dict = history.ToDict();
        Assert.Equal(0, dict["party"]);
        Assert.Equal("joined", dict["event"]);
        Assert.Equal(time.ToString("o"), dict["time"]);
    }
}
