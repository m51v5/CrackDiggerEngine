# Security policy

## Reporting a vulnerability

Please don't open a public issue. Use [private vulnerability reporting](https://github.com/m51v5/CrackDiggerEngine/security/advisories/new) instead, with the steps to reproduce it and the version you're on.

Fixes go into the latest release only.

## What the library sends

- **One or two requests per site you search.** `FindGameAsync` sends an HTTPS `GET` to that site's WordPress API (`/wp-json/wp/v2/posts`) when it has one, and to its search page when it has none or the API fails, with your keyword in the address. `FindGameAllSitesAsync` does that for every supported site.
- **What the site sees:** your IP address, the keyword, and a User-Agent naming the library and its version, `CrackDiggerEngine/2.0.0 (+https://github.com/m51v5/CrackDiggerEngine)`.
- **Your own client:** after `UseHttpClient`, requests go through your `HttpClient` with its settings, such as a proxy or extra headers. The library adds its User-Agent only if yours has none.
- **Nothing else:** no other servers, no tracking, and nothing is saved to disk.

## What the sample app sends

The sample in `samples/` uses the library as above, plus:

- **Cover images:** one `GET` per result image, to the address the site gave, with a User-Agent naming the sample.
- **Update check:** on launch and from About, one `GET` to `api.github.com/repos/m51v5/CrackDiggerEngine/releases/latest`. It only compares version numbers and never downloads anything.
- Links open in your default browser, and only `http` and `https` links are opened.

## Handling results

Titles, links and image addresses come straight from third-party pages. Treat them as untrusted input: escape titles before showing them in HTML, and check links before opening or downloading anything from them.
