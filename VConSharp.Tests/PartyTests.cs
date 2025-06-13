namespace VConSharp.Tests;

public class PartyTests
{
    [Fact]
    public void ShouldCreatePartyWithMinimalProperties()
    {
        // Arrange & Act
        var party = new Party(new Dictionary<string, object>
        {
            ["tel"] = "+1234567890",
            ["name"] = "John Doe",
        });

        // Assert
        Assert.Equal("+1234567890", party.Tel);
        Assert.Equal("John Doe", party.Name);

        var dict = party.ToDict();
        Assert.Equal("+1234567890", dict["tel"]);
        Assert.Equal("John Doe", dict["name"]);
    }

    [Fact]
    public void ShouldCreatePartyWithAllProperties()
    {
        // Arrange
        var civicAddress = new CivicAddress
        {
            Country = "US",
            Locality = "New York",
            Region = "NY",
            Postcode = "10001",
            Street = "123 Main St",
        };

        // Act
        var party = new Party(new Dictionary<string, object>
        {
            ["tel"] = "+1234567890",
            ["stir"] = "stir-id",
            ["mailto"] = "john@example.com",
            ["name"] = "John Doe",
            ["validation"] = "validated",
            ["gmlpos"] = "40.7128,-74.0060",
            ["civicaddress"] = civicAddress,
            ["uuid"] = "test-uuid",
            ["role"] = "customer",
            ["contact_list"] = "contacts",
            ["meta"] = new Dictionary<string, object> { ["key"] = "value" },
        });

        // Assert
        Assert.Equal("+1234567890", party.Tel);
        Assert.Equal("stir-id", party.Stir);
        Assert.Equal("john@example.com", party.Mailto);
        Assert.Equal("John Doe", party.Name);
        Assert.Equal("validated", party.Validation);
        Assert.Equal("40.7128,-74.0060", party.Gmlpos);
        Assert.NotNull(party.CivicAddress);
        Assert.Equal("US", party.CivicAddress?.Country);
        Assert.Equal("test-uuid", party.Uuid);
        Assert.Equal("customer", party.Role);
        Assert.Equal("contacts", party.ContactList);
        Assert.NotNull(party.Meta);
        Assert.Equal("value", party.Meta?["key"]);
    }

    [Fact]
    public void ShouldHandleUndefinedPropertiesInToDict()
    {
        // Arrange
        var party = new Party(new Dictionary<string, object>
        {
            ["tel"] = "+1234567890",
        });

        // Act
        var dict = party.ToDict();

        // Assert
        Assert.Equal("+1234567890", dict["tel"]);
        Assert.False(dict.ContainsKey("name"));
    }
}
