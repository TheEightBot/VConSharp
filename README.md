# VConSharp

A .NET library for creating and managing vCons (Virtual Conversations).

[![Build, Test and Publish](https://github.com/TheEightBot/VConSharp/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/TheEightBot/VConSharp/actions/workflows/build-and-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/VConSharp.svg)](https://www.nuget.org/packages/VConSharp/)

## Overview

VConSharp is a .NET implementation of the vCon specification, converted from the original [vcon-js](https://github.com/vcon-dev/vcon-js) TypeScript library. It allows you to create, manipulate, and analyze virtual conversations in C#.

## Installation

```bash
dotnet add package VConSharp
```

## Usage

### Creating a new vCon

```csharp
using VConSharp;

// Create a new vCon
var vcon = VCon.BuildNew();

// Add parties
var party1 = new Party(new Dictionary<string, object>
{
    ["tel"] = "+1234567890",
    ["name"] = "John Doe"
});

var party2 = new Party(new Dictionary<string, object>
{
    ["tel"] = "+0987654321",
    ["name"] = "Jane Smith"
});

vcon.AddParty(party1);
vcon.AddParty(party2);

// Add a dialog
var dialog = new Dialog(
    "text/plain", 
    DateTime.UtcNow, 
    new[] { 0, 1 }
);

dialog.Body = "Hello, this is a conversation!";
dialog.Mimetype = "text/plain";

vcon.AddDialog(dialog);

// Convert to JSON
string json = vcon.ToJson();
```

### Loading from JSON

```csharp
using VConSharp;

string jsonString = "..."; // Your vCon JSON string
var vcon = VCon.BuildFromJson(jsonString);
```

### Working with Attachments

```csharp
using VConSharp;

var vcon = VCon.BuildNew();

// Add an attachment
var attachment = vcon.AddAttachment(
    "application/pdf",
    "base64EncodedContent",
    Encoding.Base64
);

// Find an attachment by type
var pdfAttachment = vcon.FindAttachmentByType("application/pdf");
```

### Working with Analysis

```csharp
using VConSharp;

var vcon = VCon.BuildNew();

// Add analysis
vcon.AddAnalysis(new Dictionary<string, object>
{
    ["type"] = "sentiment",
    ["dialog"] = 0, // Reference to dialog index
    ["vendor"] = "sentiment-analyzer",
    ["body"] = new Dictionary<string, object>
    {
        ["score"] = 0.8,
        ["label"] = "positive"
    }
});

// Find analysis by type
var sentimentAnalysis = vcon.FindAnalysisByType("sentiment");
```

### Working with Tags

```csharp
using VConSharp;

var vcon = VCon.BuildNew();

// Add a tag
vcon.AddTag("category", "support");

// Get a tag
string category = vcon.GetTag("category");
```

## API Reference

### VCon

The main class for working with vCons.

#### Methods

- `static BuildNew()`: Creates a new vCon
- `static BuildFromJson(jsonString)`: Creates a vCon from JSON
- `AddParty(party)`: Adds a party to the vCon
- `AddDialog(dialog)`: Adds a dialog to the vCon
- `AddAttachment(type, body, encoding)`: Adds an attachment
- `AddAnalysis(parameters)`: Adds analysis data
- `AddTag(tagName, tagValue)`: Adds a tag
- `FindPartyIndex(by, val)`: Finds a party index
- `FindDialog(by, val)`: Finds a dialog
- `FindAttachmentByType(type)`: Finds an attachment by type
- `FindAnalysisByType(type)`: Finds analysis by type
- `GetTag(tagName)`: Gets a tag value
- `ToJson()`: Converts the vCon to JSON
- `Sign(privateKey)`: Signs the vCon with JWS
- `Verify(publicKey)`: Verifies the vCon signature
- `GenerateKeyPair()`: Generates a new RSA key pair for signing

### Party

Class for representing parties in a vCon.

#### Properties

- `Tel`: Telephone number
- `Stir`: STIR identifier
- `Mailto`: Email address
- `Name`: Display name
- `Validation`: Validation information
- `Gmlpos`: GML position
- `CivicAddress`: Civic address
- `Uuid`: UUID
- `Role`: Role
- `ContactList`: Contact list
- `Meta`: Additional metadata

### Dialog

Class for representing dialogs in a vCon.

#### Properties

- `Type`: Dialog type
- `Start`: Start time
- `Parties`: Party indices
- `Originator`: Originator party index
- `Mimetype`: MIME type
- `Filename`: Filename
- `Body`: Dialog content
- `Encoding`: Content encoding
- `Url`: External URL
- `Signature`: Digital signature
- `Duration`: Duration in seconds
- `Meta`: Additional metadata

#### Methods

- `AddExternalData(url, filename, mimetype)`: Adds external data
- `AddInlineData(body, filename, mimetype)`: Adds inline data
- `IsExternalData()`: Checks if dialog has external data
- `IsInlineData()`: Checks if dialog has inline data
- `IsText()`: Checks if dialog is text
- `IsAudio()`: Checks if dialog is audio
- `IsVideo()`: Checks if dialog is video
- `IsEmail()`: Checks if dialog is email

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Running Examples

```bash
dotnet run --project VConSharp.Examples/VConSharp.Examples.csproj
```

## Contributing

Please see our [contributing guidelines](CONTRIBUTING.md) for how to contribute to this project.

## Versioning and Releases

We use [SemVer](http://semver.org/) for versioning. The GitHub Actions workflow will automatically build, test, and publish new releases to NuGet.org when you create a tag with the format `vX.Y.Z`.

To create a new release:

1. Ensure all tests pass
2. Create a tag: `git tag v1.0.0`
3. Push the tag to GitHub: `git push origin v1.0.0`

The version number in the NuGet package will be derived from the tag name.

## License

MIT
