<div align="center">

# CrackDiggerEngine

**Search game listings on several public game sites from .NET.**

[![NuGet](https://img.shields.io/nuget/v/CrackDiggerEngine)](https://www.nuget.org/packages/CrackDiggerEngine)
[![Downloads](https://img.shields.io/nuget/dt/CrackDiggerEngine)](https://www.nuget.org/packages/CrackDiggerEngine)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![CI](https://github.com/m51v5/CrackDiggerEngine/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/m51v5/CrackDiggerEngine/actions/workflows/ci.yml)

© 2025–present · [@m51v5](https://github.com/m51v5) · [m51v5.com](https://m51v5.com)

</div>

---

CrackDiggerEngine is a .NET library that searches game listing sites for a keyword and returns each result's title, link and image. Search one site, a few, or all of them at once.

## Features

- **Several sites, one API:** pick a site from an enum, or search them all.
- **Clean results:** every result has a title, a link and an image address, never `null`.
- **Errors as data:** a site that fails returns `isSuccess = false` and an `ErrorMessage`, so one broken site doesn't stop the others.
- **Ready for JSON:** map results to dictionaries in one call.
- **Light:** one dependency, HtmlAgilityPack.

---

## Install

```
dotnet add package CrackDiggerEngine
```

Needs .NET 8 or newer. Package page: [nuget.org/packages/CrackDiggerEngine](https://www.nuget.org/packages/CrackDiggerEngine).

---

## Quick start

```csharp
using CrackDigger;

var result = await CrackDiggerEngine.FindGameAsync(CrackDiggerEngine.enSiteUri.ovaGamesDotCom, "witcher 3");

if (result.isSuccess)
{
    foreach (var game in result.Data!)
    {
        Console.WriteLine($"{game.Title}: {game.GameLink}");
    }
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

Search every site at once:

```csharp
var all = await CrackDiggerEngine.FindGameAllSitesAsync("witcher 3");

foreach (var (site, games) in all)
{
    Console.WriteLine($"{site}: {(games.isSuccess ? games.Data!.Count() + " results" : games.ErrorMessage)}");
}
```

---

## Supported sites

| Enum value | Site | Status (2026-11-07) |
|---|---|---|
| `fitgirl_repacksDoteSite` | fitgirl-repacks.site | ✅ Works, no images |
| `apunkagamesDotCom` | apunkagames.com | ✅ Works |
| `mrpcgamerDotNet` | mrpcgamer.net | ✅ Works |
| `ovaGamesDotCom` | ovagames.com | ✅ Works |
| `steamUnlockedDotPro` | steamunlocked.pro | ⚠️ Returns results unrelated to the search |
| `cracked_gamesDotOrg` | cracked-games.org | ❌ The site's layout changed; nothing is found |
| `steamripDotCom` | steamrip.com | ❌ Blocked by the site's bot protection |

`CrackDiggerEngine.getSuporttedSites` returns the same list as a dictionary of enum value to domain.

---

## API

All members are on the static class `CrackDiggerEngine`, in the `CrackDigger` namespace.

### Search

| Method | Returns |
|---|---|
| `FindGameAsync(enSiteUri site, string keyword)` | `Task<clsGames>`: one site's results |
| `FindGameFilteredSitesAsync(List<enSiteUri> sites, string keyword)` | `Task<Dictionary<enSiteUri, clsGames>>`: the listed sites, each searched once |
| `FindGameAllSitesAsync(string keyword)` | `Task<Dictionary<enSiteUri, clsGames>>`: every supported site |

Sites are searched one after another. Each search sends one request to that site.

### Results

**`clsGames`**, one site's search:

| Property | Meaning |
|---|---|
| `isSuccess` | `true` if the page loaded and results were found |
| `ErrorMessage` | Why it failed, when `isSuccess` is `false` |
| `SearchLink` | The search address that was requested |
| `SiteUrl` | The site's domain |
| `Data` | The results, as `IEnumerable<clsGameDataObject>`; `null` when it failed |

**`clsGameDataObject`**, one result: `Title`, `GameLink`, `ImageLink`. Each is an empty string when the site didn't provide it.

Both classes are created by the library only.

### Mapping

| Method | Returns |
|---|---|
| `MapAllGamesDataAsync(IEnumerable<clsGameDataObject> data)` | `Task<IEnumerable<Dictionary<string, string>>>` |
| `MapSingleGameDataAsync(clsGameDataObject data)` | `Task<Dictionary<string, string>>` |

Each dictionary has the keys `title`, `link` and `image`:

```csharp
var rows = await CrackDiggerEngine.MapAllGamesDataAsync(result.Data!);
string json = System.Text.Json.JsonSerializer.Serialize(rows);
```

---

## Privacy

Each search sends one request to that site's search page, with your keyword in the address and a User-Agent naming the library. Nothing goes anywhere else, and nothing is saved. Results come from third-party pages, so treat them as untrusted input. Details are in [SECURITY.md](SECURITY.md).

---

## Documentation

- [Development guide](docs/DEVELOPMENT.md): building, checks, adding or fixing a site, and publishing a release.

---

## Build from source

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
git clone https://github.com/m51v5/CrackDiggerEngine.git
cd CrackDiggerEngine
dotnet build CrackDiggerEngine.sln
```

---

## Notes

- **Moving from `CrackDiggerEngineByM51V5`:** that package ID is deprecated. Run `dotnet remove package CrackDiggerEngineByM51V5`, then `dotnet add package CrackDiggerEngine`, and change `using CrackDiggerEngineByM51V5;` to `using CrackDigger;`. Class and method names are the same.
- Sites change their pages without notice, so a site can stop working between releases. Check `isSuccess` and `ErrorMessage` on every result.

---

## Disclaimer

This library works like a search engine: it shows only information that the sites make public. It doesn't claim ownership of that information, which belongs to its owners. It stores nothing and isn't meant to be used against intellectual property or DRM rules.

---

## Contributing and support

- Found a bug, a broken site, or have an idea? [Open an issue](https://github.com/m51v5/CrackDiggerEngine/issues/new/choose).
- Want to help? Read the [contributing guide](CONTRIBUTING.md) and the [code of conduct](https://github.com/m51v5/.github/blob/main/CODE_OF_CONDUCT.md).
- Found a security problem? See [SECURITY.md](SECURITY.md).
- Other ways to reach me: [m51v5.com/contact](https://m51v5.com/contact).

---

## License

[MIT](LICENSE) © [@m51v5](https://m51v5.com)
