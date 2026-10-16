# RainbowSense

Visual Studio 2026 extension that improves IntelliSense in Blazor and Razor markup, with complete knowledge of the
Blazor component model and of the DevExpress Blazor 26 libraries.

## Features

* Razor directive completion: `@page`, `@inject`, `@code`, `@layout`, `@rendermode`, `@attribute`, `@typeparam`, ...
* Completion of built-in Blazor components (`InputText`, `EditForm`, `AuthorizeView`, `QuickGrid`, `Router`, ...) with
  their real parameters taken from the ASP.NET Core assemblies.
* DevExpress Blazor component tags (300 components of DevExpress.Blazor 26.1.5) with category based ordering.
* Attribute completion inside a component tag: every public parameter of the component with its declared type, for
  example `PageSize`, `ShowGroupPanel`, `Data`, `Columns`, plus `@bind-Value`, `@ref`, `@key`, `class`, `style`.
* Snippets that write ready to use markup for the most common components (`DxGrid` with a column, `DxFormLayout` with an
  item, `EditForm` with validation, ...).
* DevExpress namespaces offered inside `@using` directives.
* Documentation of every item taken from the XML documentation files of the shipped libraries.

## Repository layout

| Path | Description |
| --- | --- |
| `src/RainbowSense.VisualStudio` | VSIX project, `net472`, MEF completion providers |
| `src/RainbowSense.VisualStudio/Completion/Catalogs` | Generated catalogs (components, members, namespaces, snippets) |
| `tests/RainbowSense.Tests` | xUnit tests: catalogs verified against the real DevExpress assemblies, IntelliSense behaviour |
| `samples/DevExpressBlazorMock` | Razor class library that uses DevExpress.Blazor 26.1.5 and the Blazor form controls |

## Catalogs

The catalogs are generated from the real assemblies, not hand written:

* `DevExpressComponentsCatalog` - every public non abstract component of `DevExpress.Blazor` 26.1 with the summary from the
  shipped XML documentation.
* `DevExpressMembersCatalog` - every public parameter of every component, including inherited ones (3359 entries).
* `RazorComponentsCatalog` / `RazorMembersCatalog` - the ASP.NET Core Blazor components and their parameters.

Regenerating the catalogs requires the DevExpress nupkg and the ASP.NET Core reference assemblies; the tests verify the
result against the assemblies loaded by `DevExpress.Blazor 26.1.5`.

## Build and test

```powershell
dotnet build RainbowSense.sln
dotnet test tests/RainbowSense.Tests/RainbowSense.Tests.csproj
dotnet build samples/DevExpressBlazorMock/DevExpressBlazorMock.csproj
```

The VSIX is produced at `src/RainbowSense.VisualStudio/bin/Debug/net472/RainbowSense.VisualStudio.vsix`.

## Settings

Settings are stored in `%LOCALAPPDATA%\RainbowSense\settings.txt` and control which providers contribute, the sort order
and the maximum number of items.

## License

MIT