<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# VConSharp Project Instructions

VConSharp is a .NET library for creating and managing Virtual Conversations (vCons).

## Code Structure

- `VConSharp`: The main library containing the core functionality
  - `Enums.cs`: Defines enumerations used in the library
  - `Models.cs`: Contains various model classes (Attachment, PartyHistory, etc.)
  - `Party.cs`: Represents a participant in a conversation
  - `Dialog.cs`: Represents a dialog element in a conversation
  - `VCon.cs`: The main class for managing vCons

- `VConSharp.Examples`: Example projects demonstrating usage
  - `SimpleExample.cs`: A simple example of vCon usage
  - `AudioVideoExample.cs`: Example with audio and video dialogs

- `VConSharp.Tests`: Tests for the library components

## Development Guidelines

1. Follow C# coding conventions and XML documentation practices
2. Ensure all tests pass before committing changes
3. Maintain compatibility with the original vcon-js library's API where reasonable
4. Use nullable reference types appropriately
5. Include proper XML documentation for public APIs

## References

- Original vcon-js library: https://github.com/vcon-dev/vcon-js
