using CrackDigger.SitesUri;
using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger
{
    public static class CrackDiggerEngine
    {
        #region Supported Sites
        /// <summary>
        ///     Those are the supported sites in this library,
        ///     which you will need to send it as parameter to get data from.
        /// </summary>
        public enum enSiteUri
        {
            steamripDotCom,
            cracked_gamesDotOrg,
            fitgirl_repacksDoteSite,
            apunkagamesDotCom,
            mrpcgamerDotNet,
            ovaGamesDotCom,
            steamUnlockedDotPro,
        }

        /// <summary>
        ///     Will give all supported sites in this library with its enums.
        ///     Like this : { "CrackDiggerEngine.enSiteUri.XXX" : "SiteDomain" }
        /// </summary>
        public static Dictionary<enSiteUri, string> getSuporttedSites => new()
        {
            { enSiteUri.steamripDotCom,  ClsSteamripDotCom.siteUrl },
            { enSiteUri.cracked_gamesDotOrg, clsCrackedGamesDotOrg.siteUrl  },
            { enSiteUri.fitgirl_repacksDoteSite, clsFitgirlRepacksDotSite.siteUrl },
            { enSiteUri.apunkagamesDotCom, clsApunkagameDotCom.siteUrl },
            { enSiteUri.mrpcgamerDotNet, clsMrpcgamerDotNet.siteUrl },
            { enSiteUri.ovaGamesDotCom, clsOvagamesDotCom.siteUrl },
            { enSiteUri.steamUnlockedDotPro, clsSteamunlockedDotPro.siteUrl },
        };

        /// <summary>
        ///     Those are the object that will be created when you choose the site from
        ///     enum and send it as parameter.
        /// </summary>
        private static readonly Dictionary<enSiteUri, Func<ISiteInfo>> SiteFactories = new()
        {
            { enSiteUri.steamripDotCom, () => new ClsSteamripDotCom() },
            { enSiteUri.cracked_gamesDotOrg, () => new clsCrackedGamesDotOrg() },
            { enSiteUri.fitgirl_repacksDoteSite, () => new clsFitgirlRepacksDotSite() },
            { enSiteUri.apunkagamesDotCom, () => new clsApunkagameDotCom() },
            { enSiteUri.mrpcgamerDotNet, () => new clsMrpcgamerDotNet() },
            { enSiteUri.ovaGamesDotCom, () => new clsOvagamesDotCom() },
            { enSiteUri.steamUnlockedDotPro, () => new clsSteamunlockedDotPro() },
        };
        #endregion

        #region Objects
        /// <summary>
        ///     Single game info object with : title, link, image.
        /// </summary>
        public class clsGameDataObject
        {
            public readonly string Title;
            public readonly string GameLink;
            public readonly string ImageLink;

            internal clsGameDataObject(string? title, string? gameLink, string? imageLink)
            {
                Title = title ?? string.Empty;
                GameLink = gameLink ?? string.Empty;
                ImageLink = imageLink ?? string.Empty;
            }
        }

        /// <summary>
        ///     All the games from single site with info : list of games, is success, 
        ///     error message if not success, search link (link), site url
        /// </summary>
        public class clsGames
        {
            public bool isSuccess { get; set; }
            public string? ErrorMessage { get; set; }
            public string? SearchLink { get; set; }
            public string? SiteUrl { get; set; }
            public IEnumerable<clsGameDataObject>? Data { get; set; }

            internal clsGames() { }
        }
        #endregion

        #region HTTP Client
        // One client for all searches, so searching many sites doesn't open a connection pool each time.
        private static volatile HttpClient Http = CreateClient();

        /// <summary>
        ///     The User-Agent the library sends, naming itself and its version.
        /// </summary>
        public static string UserAgent { get; } = BuildUserAgent();

        /// <summary>
        ///     The client every search uses. Change its headers or timeout here,
        ///     or replace it with <see cref="UseHttpClient"/>.
        /// </summary>
        public static HttpClient HttpClient => Http;

        /// <summary>
        ///     Makes every later search use your client, for example one with a proxy, a custom handler or a test handler.
        ///     The library never disposes it. If it has no User-Agent, the library's is added; some sites refuse requests without one.
        /// </summary>
        /// <param name="client"> The client to use from now on. </param>
        public static void UseHttpClient(HttpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            if (client.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            }
            Http = client;
        }

        private static string BuildUserAgent()
        {
            Version version = typeof(CrackDiggerEngine).Assembly.GetName().Version ?? new Version(0, 0, 0);
            return $"CrackDiggerEngine/{version.Major}.{version.Minor}.{version.Build} (+https://github.com/m51v5/CrackDiggerEngine)";
        }

        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(BuildUserAgent());
            return client;
        }
        #endregion

        #region Find Game Methods

        /// <summary>
        ///     Find a game in any supported site.
        /// </summary>
        /// <param name="siteUri"> Site you want to search into. </param>
        /// <param name="keyword"> the game you want to find. </param>
        /// <returns>
        ///     Object of custom class "clsGames" which is avilable in this library.
        ///     Use it by calling "CrackDiggerEngine.clsGames".
        /// </returns>
        public static async Task<clsGames> FindGameAsync(enSiteUri siteUri, string keyword)
        {
            var Games = new clsGames();

            // Get Site Info
            if (!SiteFactories.TryGetValue(siteUri, out Func<ISiteInfo>? siteFunc))
            {
                Games.isSuccess = false;
                Games.ErrorMessage = "Catched error : curropted site info.";

                return Games;
            }

            ISiteInfo site = siteFunc!();

            // The site's own search page: what a person would open to see the same results.
            Games.SiteUrl = site.siteUri;
            Games.SearchLink = site.protocol + site.siteUri + site.searchParameter + Uri.EscapeDataString(keyword);

            // WordPress API first: it doesn't change with the site's theme.
            string? apiError = null;
            if (site.wordPress is WordPressApi api)
            {
                try
                {
                    Games.Data = await WordPressSearch.SearchAsync(Http, api, keyword);
                    Games.isSuccess = true;
                    return Games;
                }
                catch (Exception ex)
                {
                    apiError = ex.Message;
                }
            }

            // Then the search page.
            try
            {
                Games.Data = await SearchPageAsync(site, Games.SearchLink);
                Games.isSuccess = true;
            }
            catch (Exception ex)
            {
                Games.isSuccess = false;
                Games.ErrorMessage = apiError == null
                    ? "Catched error : " + ex.Message
                    : $"Catched error : API: {apiError} Search page: {ex.Message}";
            }
            return Games;
        }

        // Reads results from the site's search page; throws when there are none or search doesn't work.
        private static async Task<List<clsGameDataObject>> SearchPageAsync(ISiteInfo site, string searchLink)
        {
            string htmlCode = await Http.GetStringAsync(searchLink);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlCode);

            if (SiteText.IsHomePage(document))
            {
                throw new InvalidOperationException("site search not available: the site answered with its home page");
            }

            string xpath = string.IsNullOrEmpty(site.mainNodeDataName)
                ? $"//{site.mainNodeType}"
                : $"//{site.mainNodeType}[contains(@{site.mainNodeDataName}, '{site.mainNodeDataValue}')]";
            HtmlNodeCollection result = document.DocumentNode.SelectNodes(xpath)
                ?? throw new InvalidOperationException("empty result");

            string baseUrl = site.protocol + site.siteUri;
            List<clsGameDataObject> data = new List<clsGameDataObject>();
            foreach (HtmlNode item in result)
            {
                clsGameDataObject? gameData = site.GetSingleGameData(item);
                if (gameData != null)
                {
                    data.Add(SiteText.Normalize(gameData, baseUrl));
                }
            }
            return data;
        }

        /// <summary>
        ///     Search in multiple sites
        /// </summary>
        /// <param name="sitesUri">
        ///     List of sites you want to search into.
        /// </param>
        /// <param name="keyword">the game you want to find.</param>
        /// <returns> Multiple sites results as "Dictionary<enSiteUri, clsGames>". </returns>
        public static async Task<Dictionary<enSiteUri, clsGames>> FindGameFilteredSitesAsync(List<enSiteUri> sitesUri, string keyword)
        {
            Dictionary<enSiteUri, clsGames> games = new Dictionary<enSiteUri, clsGames>();

            foreach (enSiteUri site in sitesUri)
            {
                if (!games.ContainsKey(site))
                {
                    clsGames singleSiteGames = await FindGameAsync(site, keyword);
                    games.Add(site, singleSiteGames);
                }
            }

            return games;
        }

        /// <summary>
        ///     Search in All sites
        /// </summary>
        /// <param name="keyword">the game you want to find.</param>
        /// <returns> Multiple sites results as "Dictionary<enSiteUri, clsGames>". </returns>
        public static async Task<Dictionary<enSiteUri, clsGames>> FindGameAllSitesAsync(string keyword)
        {
            Dictionary<enSiteUri, clsGames> games = new Dictionary<enSiteUri, clsGames>();

            foreach (enSiteUri site in getSuporttedSites.Keys)
            {
                clsGames singleSiteGames = await FindGameAsync(site, keyword);
                games.Add(site, singleSiteGames);
            }

            return games;
        }
        #endregion

        #region Mapping

        /// <summary>
        ///     When getting "clsGames" object, the data will be a IEnumerable of clsGameDataObject.
        ///     this method help you to convert it into a dictionary (which is basically a josn)
        /// </summary>
        /// <param name="data"> 
        ///     "clsGame.Data" 
        ///     which is "IEnumerable<CrackDiggerEngine.clsGameDataObject>"
        /// </param>
        /// <returns>
        ///     "IEnumerable<Dictionary<string, string>>" which is basically a josn
        /// </returns>
        public static Task<IEnumerable<Dictionary<string, string>>> MapAllGamesDataAsync(IEnumerable<CrackDiggerEngine.clsGameDataObject> data)
        {
            var newData = new List<Dictionary<string, string>>();

            foreach (var item in data)
            {
                newData.Add(
                    new Dictionary<string, string>
                        {
                            { "title", item.Title },
                            { "link", item.GameLink },
                            { "image", item.ImageLink }
                        }
                    );
            }

            return Task.FromResult<IEnumerable<Dictionary<string, string>>>(newData);
        }

        /// <summary>
        ///     convert a "CrackDiggerEngine.clsGameDataObject" to "Dictionary<string, string>"
        /// </summary>
        /// <param name="data">
        ///     "CrackDiggerEngine.clsGameDataObject"
        /// </param>
        /// <returns>
        ///     "Dictionary<string, string>"
        /// </returns>
        public static Task<Dictionary<string, string>> MapSingleGameDataAsync(CrackDiggerEngine.clsGameDataObject data)
        {
            return Task.FromResult(new Dictionary<string, string>
            {
                { "title", data.Title },
                { "link", data.GameLink },
                { "image", data.ImageLink }
            });
        }
        #endregion
    }
}
