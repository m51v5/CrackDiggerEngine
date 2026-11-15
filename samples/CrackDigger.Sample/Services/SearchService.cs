using static CrackDigger.CrackDiggerEngine;

namespace CrackDigger.Sample.Services
{
    /// <summary>One supported site: its enum value and domain.</summary>
    public sealed record SiteInfo(enSiteUri Site, string Domain);

    /// <summary>The only class in the sample that calls the library.</summary>
    public static class SearchService
    {
        /// <summary>Every site the library supports, in the library's order.</summary>
        public static IReadOnlyList<SiteInfo> Sites { get; } =
            getSuporttedSites.Select(pair => new SiteInfo(pair.Key, pair.Value)).ToList();

        /// <summary>Searches one site off the UI thread; the library reports failures in the result instead of throwing.</summary>
        public static Task<clsGames> SearchAsync(enSiteUri site, string keyword) => Task.Run(() => FindGameAsync(site, keyword));
    }
}
