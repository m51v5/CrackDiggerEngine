# Security policy

## Reporting a vulnerability

Please don't open a public issue. Use [private vulnerability reporting](https://github.com/m51v5/CrackDiggerEngine/security/advisories/new) instead, with the steps to reproduce it and the version you're on.

Fixes go into the latest release only.

## What the library sends

- **One request per site you search.** `FindGameAsync` sends one HTTPS `GET` to that site's search page, with your keyword in the address. `FindGameAllSitesAsync` does that for every supported site.
- **What the site sees:** your IP address, the keyword, and a User-Agent naming the library and its version, `CrackDiggerEngine/2.0.0 (+https://github.com/m51v5/CrackDiggerEngine)`.
- **Nothing else:** no other servers, no tracking, and nothing is saved to disk.

## Handling results

Titles, links and image addresses come straight from third-party pages. Treat them as untrusted input: escape titles before showing them in HTML, and check links before opening or downloading anything from them.
