using HtmlAgilityPack;

namespace CrackDigger.SitesUri.Interfaces
{
    internal interface ISiteInfo
    {
        public string protocol { get; }
        public string siteUri { get; }
        public string searchParameter { get; }
        public string mainNodeType { get; }
        public string mainNodeDataName { get; }
        public string mainNodeDataValue { get; }

        /// <summary>The site's WordPress REST API, tried before the search page; null when it has none.</summary>
        public WordPressApi? wordPress => null;

        /// <summary>Reads one result; the engine cleans entities and makes links absolute afterwards.</summary>
        CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item);
    }
}
