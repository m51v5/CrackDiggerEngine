# Development

How to build, check and change CrackDiggerEngine, and how a release reaches nuget.org.

---

## Build

You need the .NET 8 SDK. `global.json` picks the newest 8.0 SDK you have. From the repo root:

```
dotnet build CrackDiggerEngine.sln
```

To try it, make a small console app outside the repo that references `src/CrackDiggerEngine/CrackDiggerEngine.csproj`, and call `CrackDiggerEngine.FindGameAsync` from it. Each call sends one real request to that site, so keep test runs short.

## Checks

These are what CI runs, and they must pass:

```
dotnet format CrackDiggerEngine.sln --verify-no-changes
dotnet build CrackDiggerEngine.sln -c Release -warnaserror
dotnet pack src/CrackDiggerEngine/CrackDiggerEngine.csproj -c Release -o artifacts
```

`dotnet format CrackDiggerEngine.sln` fixes formatting, following `.editorconfig`. There are no automated tests, because every site lives on the internet and changes without notice.

## Publish a release

Pushing to `main` runs CI when the commit changes what gets built: `src/`, the `.sln`, `global.json`, `.editorconfig` or `ci.yml` itself. Pushing a `v*` tag runs the Release workflow, which:

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
  SitesUri/Interfaces/ISiteInfo.cs   what each site provides: address, search path, result node, parser
  SitesUri/cls*.cs               one class per site
  icon.jpg                       package icon
```

## Adding or fixing a site

1. Save one search results page from the site and find the element that wraps each result.
2. In the site's class, set `searchParameter` and the `mainNode...` values to match that element.
3. In `GetSingleGameData`, read the title, link and image from it. Return `null` to skip an element that isn't a result.
4. For a new site: add an `enSiteUri` value and a row in both dictionaries in `CrackDiggerEngine.cs`, and a row in the README's site table.
5. Search it once from your console app.

A site that blocks plain requests, for example with a Cloudflare challenge, isn't supported. Don't work around it.
