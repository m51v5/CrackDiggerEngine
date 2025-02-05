using CrackDiggerEngineByM51V5.SitesUri;
using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5
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

            internal clsGameDataObject(string title, string gameLink, string imageLink)
            {
                Title = title;
                GameLink = gameLink;
                ImageLink = imageLink;
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
            string keyLink = site.protocol + site.siteUri + site.searchParameter + keyword;

            // Start
            List<clsGameDataObject> data = new List<clsGameDataObject>();
            Games.SiteUrl = site.siteUri;
            Games.SearchLink = keyLink;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Intialize
                    string htmlCode = await client.GetStringAsync(keyLink);

                    // Load
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(htmlCode);

                    HtmlNodeCollection? result;

                    // Get main nodes
                    if (string.IsNullOrEmpty(site.mainNodeDataName))
                    {
                        result = document.DocumentNode.SelectNodes($"//{site.mainNodeType}");
                    }
                    else
                    {
                        result = document.DocumentNode.SelectNodes($"//{site.mainNodeType}[contains(@{site.mainNodeDataName}, '{site.mainNodeDataValue}')]");
                    }

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            clsGameDataObject gameData = await site.GetSingleGameDataAsync(item);

                            if (gameData != null)
                            {
                                data.Add(gameData);
                            }
                        }

                        Games.isSuccess = true;
                        Games.Data = data;
                    }
                    else
                    {
                        Games.isSuccess = false;
                        Games.ErrorMessage = "Catched error : empty result";
                    }
                }
                catch (Exception ex)
                {
                    Games.isSuccess = false;
                    Games.ErrorMessage = "Catched error : " + ex.Message;
                }
            }
            return Games;
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
        public static async Task<IEnumerable<Dictionary<string, string>>> MapAllGamesDataAsync(IEnumerable<CrackDiggerEngine.clsGameDataObject> data)
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

            return newData;
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
        public static async Task<Dictionary<string, string>> MapSingleGameDataAsync(CrackDiggerEngine.clsGameDataObject data)
        {
            return new Dictionary<string, string>
            {
                { "title", data.Title },
                { "link", data.GameLink },
                { "image", data.ImageLink }
            };
        }
        #endregion
    }
}
