using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5.SitesUri.Interfaces
{
    internal interface ISiteInfo
    {
        public static string siteUrl { get; }
        public string protocol { get; }
        public string siteUri { get; }
        public string searchParameter { get; }
        public string mainNodeType { get; }
        public string mainNodeDataName { get; }
        public string mainNodeDataValue { get; }

        Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode? item);
    }
}
