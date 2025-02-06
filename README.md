# CrackDiggerEngine v1.2.0

## Overview
The `CrackDiggerEngine` library is designed to search for game information across several supported sites. It offers methods to retrieve game details and convert data to JSON-like structures.

---

## Table of Contents
- [Setup Library](#setup)
- [Supported Sites](#supported-sites)
- [Classes and Enums](#classes-and-enums)
  - [enSiteUri](#ensiteuri)
  - [clsGameDataObject](#clsgamedataobject)
  - [clsGames](#clsgames)
- [Methods](#methods)
  - [FindGameAsync](#findgameasync)
  - [MapAllGamesDataAsync](#mapallgamesdataasync)
  - [MapSingleGameDataAsync](#mapsinglegamedataasync)

---

## Setup

Download from [nuget manager](https://www.nuget.org/packages/CrackDiggerEngineByM51V5/)

Import the library with:
```csharp
using CrackDiggerEngineByM51V5;
```
and use the main class `CrackDiggerEngine` to use it.

---

## Supported Sites
This library supports searching on the following sites:

| Enum Value | Site Domain |
|------------|-------------|
| `steamripDotCom` | steamrip.com |
| `cracked_gamesDotOrg` | cracked-games.org |
| `fitgirl_repacksDoteSite` | fitgirl-repacks.site |
| `apunkagamesDotCom` | apunkagames.com |
| `mrpcgamerDotNet` | mrpcgamer.net |

You can access all supported sites programmatically using the `getSuporttedSites` property, which returns a dictionary.

---

## Classes and Enums

### `enSiteUri`
An enumeration representing the supported sites in this library.

Example:
```csharp
CrackDiggerEngine.enSiteUri site = CrackDiggerEngine.enSiteUri.steamripDotCom;
```

### `clsGameDataObject`
Represents a single game entry with the following properties:
- `Title`: The title of the game.
- `GameLink`: The URL to the game page.
- `ImageLink`: The URL to the game image.

### `clsGames`
Represents a collection of games retrieved from a site, along with metadata.

Properties:
- `isSuccess`: Indicates whether the search was successful.
- `ErrorMessage`: The error message if the search failed.
- `SearchLink`: The URL used to perform the search.
- `SiteUrl`: The base URL of the site.
- `Data`: A collection of `clsGameDataObject`.

**Note:** The `clsGameDataObject` and `clsGames` classes are designed to be accessed only through the library. Direct instantiation is restricted to maintain data integrity.

---

## Methods

### `FindGameAsync`
Finds a game on the specified site.

#### Parameters
- `siteUri` (`enSiteUri`): The site to search.
- `keyword` (`string`): The game keyword to search for.

#### Returns
A `Task<clsGames>` object containing the search results.

#### Example Usage
```csharp
try
{
    var result = await CrackDiggerEngine.FindGameAsync(CrackDiggerEngine.enSiteUri.steamripDotCom, "Game Name");
    if (result.isSuccess)
    {
        Console.WriteLine($"Site : {result.SiteUrl}");
        Console.WriteLine($"Search link : {result.SearchLink}");
        Console.WriteLine("Games Found :\n");
        foreach (var game in result.Data)
        {
            Console.WriteLine($"Title: {game.Title}, Link: {game.GameLink}, Image Link: {game.ImageLink}");
        }
    }
    else
    {
        Console.WriteLine($"Error: {result.ErrorMessage}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

---

### `MapAllGamesDataAsync`
Converts a collection of `clsGameDataObject` to a list of dictionaries.

#### Parameters
- `data` (`IEnumerable<clsGameDataObject>`): The game data to convert.

#### Returns
A `Task<IEnumerable<Dictionary<string, string>>>` representing the converted data.

#### Example Usage
```csharp
var convertedData = await CrackDiggerEngine.ConvertAllGamesDataAsync(result.Data);
foreach (var item in convertedData)
{
    Console.WriteLine($"Title: {item["title"]}, Link: {item["link"]}, Image: {item["image"]}");
}
```

---

### `MapSingleGameDataAsync`
Converts a single `clsGameDataObject` to a dictionary.

#### Parameters
- `data` (`clsGameDataObject`): The game data to convert.

#### Returns
A `Task<Dictionary<string, string>>` representing the converted data.

#### Example Usage
```csharp
var singleGameData = await CrackDiggerEngine.ConvertSingleGameDataAsync(game);
Console.WriteLine($"Title: {singleGameData["title"]}, Link: {singleGameData["link"]}, Image: {singleGameData["image"]}");
```

---

## Notes
- Ensure you handle exceptions when calling `FindGameAsync`, as network issues may cause errors.
- The library uses `HtmlAgilityPack` for HTML parsing.
- Always validate and sanitize inputs when using this library.

---

## Conclusion
The `CrackDiggerEngine` library provides a powerful way to search and retrieve game information from multiple sources. With its easy-to-use methods and structured data classes, developers can quickly integrate game search functionalities into their applications.

---

## Copyright
© 2025-02-01 Mustafa (@m51v5). All rights reserved.

Get In Touch: [https://m51v5.mssg.me/](https://m51v5.mssg.me/)

---

### Disclaimer
This library acts as a search engine and displays only publicly available information. We do not claim ownership of the data displayed. All rights belong to their respective owners from which the data was sourced. No data is stored or misused in violation of intellectual property or DRM policies.
