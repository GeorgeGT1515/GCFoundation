## .NET 8 ASP.NET MVC + GCFoundation + BL/DAL (One‑Pager)

This guide shows how to create a new .NET 8 ASP.NET MVC app using the latest GCFoundation NuGet packages, add a simple “Hello World” page, and set up Business and Data Access layer class libraries with Entity Framework Core.

### Prerequisites
- **.NET 8 SDK** installed
- **PowerShell** or a terminal
- Optional: **SQL Server** (or change provider) if using EF Core SqlServer

### 1) Create solution and MVC project
```powershell
mkdir GCFHello && cd GCFHello
dotnet new sln -n GCFHello
dotnet new mvc -n GCFHello.Web -f net8.0
dotnet sln add GCFHello.Web
```

### 2) Add latest GCFoundation packages (Web)
```powershell
dotnet add GCFHello.Web package GCFoundation.Common
dotnet add GCFHello.Web package GCFoundation.Components
dotnet add GCFHello.Web package GCFoundation.Security
# (Optional) Additional packages your app needs
# dotnet add GCFHello.Web package cloudscribe.Web.Navigation
# dotnet add GCFHello.Web package cloudscribe.Web.Localization
```

### 2.1) GCFoundation configuration in `Program.cs` (modular and simple)
- **What you add**: register only the modules you need, then enable the corresponding middleware. The defaults are safe and focused on GC web standards.
- **Why it’s simple**: each capability is a small, independent building block—bring in components (assets), security (CSP/policies), and session as needed.

Register GCFoundation services:
```33:37:Program.cs
// Configure GCFoundation.
builder.Services.AddGCFoundationComponents(builder.Configuration);
builder.Services.AddGCFoundationContentPolicies(builder.Configuration);
builder.Services.AddGCFoundationSession(builder.Configuration);
```

Enable GCFoundation middleware (order these with your other middleware as appropriate):
```69:75:Program.cs
// Load all javascript dependencies for GCFoundation and GCDS.
app.UseMiddleware<GCFoundationComponentsMiddleware>();

// Add GCFoundation security middleware (Add CSP).
app.UseMiddleware<GCFoundationContentPoliciesMiddleware>();
app.UseMiddleware<GCFoundationLanguageMiddleware>();
```

Optionally use the convenience extensions:
```87:91:Program.cs
// Use GCFoundation.
app.UseGCFoundationComponents();
app.UseGCFoundationContentPolicies();
app.UseGCFoundationSession();
```

Notes:
- **Components**: injects required GCDS/GCFoundation assets in a predictable way.
- **Security**: applies Content Security Policy and related headers tuned for GC web apps.
- **Session**: provides sane defaults for cookie/session handling.
- **Localization**: the sample also wires request localization and a language middleware; keep or remove based on your needs.

Localization configuration (services and middleware):
```csharp
// Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Middleware (place early in the pipeline, before MVC)
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var supportedCultures = new[] { new CultureInfo("en-CA"), new CultureInfo("fr-CA") };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-CA")
    .AddSupportedCultures("en-CA", "fr-CA")
    .AddSupportedUICultures("en-CA", "fr-CA");

app.UseRequestLocalization(localizationOptions);
// If using GCFoundation language middleware, keep it after UseRequestLocalization
app.UseMiddleware<GCFoundationLanguageMiddleware>();
```

### 3) “Hello World” page (using Business layer via DI)
Controller injects `IGreetingService` and passes its data to the view as the model:
```csharp
// GCFHello.Web/Controllers/HomeController.cs
using GCFHello.Business;
using GCFoundation.Components;
using Microsoft.AspNetCore.Mvc;

namespace GCFHello.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGreetingService _greeting;
        public HomeController(IGreetingService greeting) => _greeting = greeting;

        public IActionResult Index()
        {
            var message = _greeting.GetGreeting();

            // Set the page title via GCFoundation
            this.SetPageTitle("Home");

            return View(model: message);
        }
    }
}
```
Create the view `Views/Home/Index.cshtml` to render the greeting:
```cshtml
@model string
<h1>@Model</h1>
```

### 4) Create Business and Data projects
```powershell
dotnet new classlib -n GCFHello.Business -f net8.0
dotnet new classlib -n GCFHello.Data -f net8.0
dotnet sln add GCFHello.Business GCFHello.Data

# Project references (Web -> Business -> Data)
dotnet add GCFHello.Business reference GCFHello.Data
dotnet add GCFHello.Web reference GCFHello.Business
```

### 5) Add EF Core to Data Access layer
```powershell
dotnet add GCFHello.Data package Microsoft.EntityFrameworkCore
dotnet add GCFHello.Data package Microsoft.EntityFrameworkCore.SqlServer
dotnet add GCFHello.Data package Microsoft.EntityFrameworkCore.Tools
```
Sample `DbContext` and entity:
```csharp
// GCFHello.Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace GCFHello.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Greeting> Greetings => Set<Greeting>();
    }

    public class Greeting
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
```

### 6) Simple Business service and wire-up
```csharp
// GCFHello.Business/IGreetingService.cs
namespace GCFHello.Business
{
    public interface IGreetingService
    {
        string GetGreeting();
    }
}

// GCFHello.Business/GreetingService.cs
namespace GCFHello.Business
{
    public class GreetingService : IGreetingService
    {
        public string GetGreeting() => "Hello World";
    }
}
```
Register services and EF Core in the Web app’s `Program.cs`:
```csharp
// GCFHello.Web/Program.cs (relevant additions)
using GCFHello.Business;
using GCFHello.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// GCFoundation services can be configured as needed here
builder.Services.AddScoped<IGreetingService, GreetingService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// Localization middleware (place early in the pipeline)
var supportedCultures = new[] { new CultureInfo("en-CA"), new CultureInfo("fr-CA") };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-CA")
    .AddSupportedCultures("en-CA", "fr-CA")
    .AddSupportedUICultures("en-CA", "fr-CA");

app.UseRequestLocalization(localizationOptions);
// If using GCFoundation language middleware, keep it after UseRequestLocalization
// app.UseMiddleware<GCFoundationLanguageMiddleware>();

// ... existing pipeline/middleware ...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```
 

Add a connection string in `GCFHello.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GCFHello;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### GCFoundation settings in `appsettings.json`
Add GCFoundation-specific configuration under a `GCFoundation` section. The `AddGCFoundationComponents` and `AddGCFoundationContentPolicies` registrations will read from these values when present.

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

Notes:
- Adjust CDN usage and base URL to match your deployment (CDN vs local assets).
- CSP is strict by default; use the "Additional*Src" arrays to extend allow-lists when integrating analytics or external services.
- All keys are optional; safe defaults apply when omitted.

### 7) Build and run
```powershell
dotnet build
dotnet run --project GCFHello.Web
```

Optional EF Core migrations (if you need a database created):
```powershell
dotnet tool install -g dotnet-ef
dotnet ef migrations add InitialCreate --project GCFHello.Data --startup-project GCFHello.Web
dotnet ef database update --project GCFHello.Data --startup-project GCFHello.Web
```

You now have:
- **GCFHello.Web**: ASP.NET MVC with latest GCFoundation packages and a Hello World page
- **GCFHello.Business**: business layer with a simple greeting service
- **GCFHello.Data**: EF Core `DbContext` ready for persistence