# Contributing to VConSharp

We love your input! We want to make contributing to VConSharp as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## Development Process

We use GitHub to host code, to track issues and feature requests, as well as accept pull requests.

## Pull Requests

1. Fork the repo and create your branch from `main`.
2. If you've added code that should be tested, add tests.
3. If you've changed APIs, update the documentation.
4. Ensure the test suite passes.
5. Make sure your code follows the project's coding style.
6. Issue that pull request!

## Coding Style

* Use C# coding conventions as outlined in the [Microsoft C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
* Include XML documentation for all public APIs
* Use nullable reference types appropriately
* Keep the code structure similar to the original vcon-js library where appropriate

## License

By contributing, you agree that your contributions will be licensed under the project's MIT License.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/YOUR-USERNAME/VConSharp/tags).

To create a new release:

1. Ensure all tests pass
2. Update version information in relevant files
3. Create a tag in the format `v1.2.3`
4. Push the tag to GitHub
5. The GitHub Actions workflow will automatically build and publish the package to NuGet.org

## References

- [vCon Specification](https://github.com/vcon-dev/vcon)
- [vcon-js library](https://github.com/vcon-dev/vcon-js)
