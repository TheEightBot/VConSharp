# Setting Up User Secrets for VConSharp Examples

The VConSharp.Examples project uses the .NET user secrets feature to store API credentials securely.

## Prerequisites

Make sure you have the .NET SDK installed.

## Steps to Set Up User Secrets

1. Navigate to the VConSharp.Examples project directory:

```bash
cd /path/to/VConSharp/VConSharp.Examples
```

2. Set your API URL:

```bash
dotnet user-secrets set "root_url" "https://your-api-url"
```

3. Set your API token:

```bash
dotnet user-secrets set "api_token" "your-api-token"
```

4. Verify your secrets (optional):

```bash
dotnet user-secrets list
```

## Alternative: Using Command Line Arguments

You can also provide the API URL and token as command line arguments:

```bash
dotnet run --project VConSharp.Examples/VConSharp.Examples.csproj https://your-api-url your-api-token
```

## Notes

- User secrets are stored outside of your project directory, so they won't be committed to source control.
- In a production environment, you might want to use environment variables or a secure key vault instead of user secrets.
