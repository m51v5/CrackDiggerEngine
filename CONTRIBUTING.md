# Contributing

Thanks for helping out. Bug reports, ideas and pull requests are all welcome.

## Reporting a bug or asking for a feature

Use the [issue forms](https://github.com/m51v5/CrackDiggerEngine/issues/new/choose). For a broken site, the site, the keyword and the `ErrorMessage` usually make it quick to fix.

For a security problem, see [SECURITY.md](SECURITY.md) instead of opening an issue.

## Making a change

You'll need the .NET 8 SDK. The [development guide](docs/DEVELOPMENT.md) covers building, the checks and how the code is laid out.

1. Fork the repo and branch from `main`.
2. Make your change. For anything big, open an issue first so we can agree on the approach.
3. Run these, which must both pass:

   ```
   dotnet format CrackDiggerEngine.sln
   dotnet build CrackDiggerEngine.sln -c Release -warnaserror
   ```

4. If you changed a site's parser, search it once from a small console app and check the titles, links and images.
5. Open a pull request saying what changed and why.

## Guidelines

- **One request per search.** Don't add retries, crawling of result pages, or anything that loads a site harder than a person searching would.
- **Don't get around bot protection.** If a site blocks plain requests, the site isn't supported.
- **Keep the public API stable** within a major version.
- **Keep comments short:** one line that says why, not what.

By contributing, you agree that your work is released under the [MIT License](LICENSE). Everyone taking part is expected to follow the [code of conduct](https://github.com/m51v5/.github/blob/main/CODE_OF_CONDUCT.md).
