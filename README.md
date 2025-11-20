# GCFoundation

**GCFoundation** is a comprehensive .NET 8 ASP.NET MVC foundation designed for building Government of Canada (GC) web applications. It provides a robust starting point with pre-configured components, security policies, session management, and localization features, ensuring compliance with GC standards.

## Features

- **GCFoundation.Common**: Shared utilities and models used across the foundation.
- **GCFoundation.Components**: Pre-built UI components and helpers integrated with the Government of Canada Design System (GCDS).
- **GCFoundation.Security**: Security middleware, including Content Security Policy (CSP) and other headers tuned for GC web apps.
- **GCFoundation.Web**: The main web application project demonstrating the implementation of the foundation.

## Quick Start

This guide shows how to create a new .NET 8 ASP.NET MVC app using the latest GCFoundation NuGet packages.

### Prerequisites

- **.NET 8 SDK** installed
- **PowerShell** or a terminal

### 1. Create Solution and MVC Project

```powershell
mkdir GCFHello && cd GCFHello
dotnet new sln -n GCFHello
dotnet new mvc -n GCFHello.Web -f net8.0
dotnet sln add GCFHello.Web
```

### 2. Add GCFoundation Packages

```powershell
dotnet add GCFHello.Web package GCFoundation.Common
dotnet add GCFHello.Web package GCFoundation.Components
dotnet add GCFHello.Web package GCFoundation.Security
# (Optional) Additional packages
dotnet add GCFHello.Web package cloudscribe.Web.Navigation
dotnet add GCFHello.Web package cloudscribe.Web.Localization
```

### 3. Configuration

#### Update `Program.cs`

Register GCFoundation services and middleware.

```csharp
// Register GCFoundation services
builder.Services.AddGCFoundationComponents(builder.Configuration);
builder.Services.AddGCFoundationContentPolicies(builder.Configuration);
builder.Services.AddGCFoundationSession(builder.Configuration);

// Enable GCFoundation middleware
app.UseMiddleware<GCFoundationComponentsMiddleware>();
app.UseMiddleware<GCFoundationContentPoliciesMiddleware>();
app.UseMiddleware<GCFoundationLanguageMiddleware>();

// Or use convenience extensions
app.UseGCFoundationComponents();
app.UseGCFoundationContentPolicies();
app.UseGCFoundationSession();
```

#### Update `appsettings.json`

Add the `GCFoundation` configuration section.

```json
{
  "GCFoundation": {
    "Components": {
      "UseCdn": true,
      "CdnBaseUrl": "https://cdn.design-system.alpha.canada.ca",
      "InjectGcds": true
    },
    "ContentPolicies": {
      "ReportOnly": false,
      "ReportUri": "/csp-report",
      "AdditionalScriptSrc": [
        "https://www.googletagmanager.com",
        "https://www.google-analytics.com"
      ],
      "AdditionalStyleSrc": [],
      "AdditionalImgSrc": ["data:"],
      "AdditionalConnectSrc": []
    },
    "Localization": {
      "DefaultCulture": "en-CA",
      "SupportedCultures": ["en-CA", "fr-CA"]
    }
  }
}
```

### 4. Build and Run

```powershell
dotnet build
dotnet run --project GCFHello.Web
```

## Architecture

- **Components**: Injects required GCDS/GCFoundation assets in a predictable way.
- **Security**: Applies Content Security Policy and related headers tuned for GC web apps.
- **Session**: Provides defaults for cookie/session handling.
- **Localization**: Supports request localization and language middleware.

## License

[MIT License](LICENSE)

