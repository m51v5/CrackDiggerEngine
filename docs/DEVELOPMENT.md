# Development

How to build, check and change CrackDiggerEngine, and how a release reaches nuget.org.

---

## Build

You need the .NET 8 SDK. `global.json` picks the newest 8.0 SDK you have. From the repo root:

```
dotnet build CrackDiggerEngine.sln
```

To try it, run the sample app, which uses the library from `src/` directly (Windows only):

```
dotnet run --project samples/CrackDigger.Sample
```

Each search sends one real request to each selected site, so keep test runs short.

## Checks

These are what CI runs, and they must pass:

```
dotnet format CrackDiggerEngine.sln --verify-no-changes
dotnet build CrackDiggerEngine.sln -c Release -warnaserror
dotnet pack src/CrackDiggerEngine/CrackDiggerEngine.csproj -c Release -o artifacts
```

`dotnet format CrackDiggerEngine.sln` fixes formatting, following `.editorconfig`. There are no automated tests, because every site lives on the internet and changes without notice.

## Publish a release

Pushing to `main` runs CI when the commit changes what gets built: `src/`, `samples/`, the `.sln`, `global.json`, `.editorconfig` or `ci.yml` itself. Pushing a `v*` tag runs the Release workflow, which:

1. checks the tagged commit is on `main`
2. checks the tag matches `<Version>` in `src/CrackDiggerEngine/CrackDiggerEngine.csproj`
3. packs `CrackDiggerEngine.<version>.nupkg`
4. creates the GitHub release with the package attached
5. pushes the package to nuget.org

A version on nuget.org can never be deleted or replaced, only unlisted, so check everything before tagging.

### One-time setup on nuget.org

Done once, before the first release from this workflow:

1. **Trusted publishing,** so the workflow needs no stored API key. On nuget.org, open your account menu → **Trusted Publishing** → **Create**:
   - Repository owner: `m51v5`
   - Repository: `CrackDiggerEngine`
   - Workflow file: `release.yml`

   The workflow logs in as `m51v5` with `NuGet/login` and gets a key that lasts only for that run.
2. **The old package ID,** after `CrackDiggerEngine 2.0.0` is on nuget.org: open `CrackDiggerEngineByM51V5` → **Manage package** → **Deprecation**, select all versions, choose **Legacy**, and set the alternate package to `CrackDiggerEngine`. People using the old ID then see where it moved.

### Steps

Replace `2.1.0` with the new version.

1. Start from an up-to-date `main`, with nothing uncommitted:
   ```
   git checkout main
   git pull
   git status
   ```
2. Set `<Version>2.1.0</Version>` in `src/CrackDiggerEngine/CrackDiggerEngine.csproj`.
3. Commit and push:
   ```
   git add src/CrackDiggerEngine/CrackDiggerEngine.csproj
   git commit -m "chore: release v2.1.0"
   git push
   ```
4. Wait for CI to pass, in the Actions tab or with:
   ```
   gh run list --workflow ci.yml --limit 1
   ```
5. Tag that same commit and push the tag. Don't commit anything in between:
   ```
   git tag v2.1.0
   git push origin v2.1.0
   ```
6. Wait for the Release workflow:
   ```
   gh run list --workflow release.yml --limit 1
   ```
7. Write the notes. Generated notes only list merged pull requests. Click **Edit** on the release, or run:
   ```
   gh release edit v2.1.0 --notes-file notes.md
   ```
8. Check the package page, `https://www.nuget.org/packages/CrackDiggerEngine/2.1.0`. nuget.org takes a few minutes to index a new version.

**If the Release workflow fails,** check which step stopped it:

| Failed step | Cause | Fix |
|---|---|---|
| Check the tag is on main | The tag is on another branch | Remove the tag, tag the commit on `main` |
| Check the tag matches the version | Tag and `<Version>` differ | Remove the tag, fix `<Version>`, push, tag again |
| GitHub release | A release with that tag already exists | Delete the release on GitHub, rerun the workflow |
| NuGet login | No trusted publishing policy, or it names another repo or workflow file | Fix the policy on nuget.org, rerun the workflow |
| Push to nuget.org | The version already exists there | It's published; nothing to do. A new fix needs a new version |

To remove a tag before a rerun:

```
git tag -d v2.1.0
git push origin --delete v2.1.0
```

---

## Source layout

```
src/CrackDiggerEngine/
  CrackDiggerEngine.cs           public API: sites enum, result classes, search and map methods
  SitesUri/Interfaces/ISiteInfo.cs   what each site provides: address, search path, result node, parser, WordPress API
  SitesUri/cls*.cs               one class per site
  SitesUri/WordPressSearch.cs    search through a site's WordPress REST API
  SitesUri/SiteText.cs           entity decoding, absolute links, lazy images, home-page check
  icon.png                       package icon

samples/CrackDigger.Sample/      WPF sample, not packed or released
  Views/                         main and About windows
  ViewModels/                    MVVM with CommunityToolkit.Mvvm
  Services/SearchService.cs      the only class that calls the library
  Services/ImageLoader.cs        cover images, with the sample's User-Agent
  Services/UpdateChecker.cs      compares the library version with the latest release
  Themes/Dark.xaml               every color and style
  AppLinks.cs                    every address the sample opens or reads
```

The sample builds in CI on Linux too, through `EnableWindowsTargeting`; it only runs on Windows.

## Adding or fixing a site

1. **WordPress first.** If the site's search address is `/?s=`, open `https://<site>/wp-json/wp/v2/posts?search=test` once. If it answers with JSON, set `wordPress => new("<host>")` in the site's class. Add `ImageFromContent: true` when its posts have no featured image, so the cover comes from the first image in the post. The search page below then only runs when the API fails.
2. **Search page.** Save one search results page and find the element that wraps each result. If the site has no `/?s=`, its home page often names its search address in a schema.org `SearchAction`.
3. In the site's class, set `searchParameter` and the `mainNode...` values to match that element.
4. In `GetSingleGameData`, read the title, link and image from it. Return `null` to skip an element that isn't a result. The engine decodes entities and makes links absolute afterwards, so return them as the page has them.
5. For a new site: add an `enSiteUri` value and a row in both dictionaries in `CrackDiggerEngine.cs`, and a row in the README's site table.
6. Search it once from the sample app.

A site that blocks plain requests, for example with a Cloudflare challenge, isn't supported. Don't work around it.
