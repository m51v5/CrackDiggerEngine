using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class clsApunkagameDotCom : ISiteInfo
    {
        public string protocol => "https://";

        // The site moved from apunkagames.com and serves from www.
        public string siteUri => "www." + siteUrl;
        public static string siteUrl => "aapunkagames.com";
        public string searchParameter => "/?s=";
        public string mainNodeType => "article";
        public string mainNodeDataName => "";
        public string mainNodeDataValue => "";

        public WordPressApi? wordPress => new(siteUri);

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            string imageLink = SiteText.ImageSource(item.SelectSingleNode(".//img"));
            string? gameLink = item.SelectSingleNode(".//a")?.GetAttributeValue("href", string.Empty);
            string? title = item.SelectSingleNode(".//h2")?.SelectSingleNode(".//a")?.InnerText;

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
