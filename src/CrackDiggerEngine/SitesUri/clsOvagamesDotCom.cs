using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class clsOvagamesDotCom : ISiteInfo
    {
        public static string siteUrl => "ovagames.com";
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public string searchParameter => "/?s=";
        public string mainNodeType => "div";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "post-inside";

        public WordPressApi? wordPress => new(siteUrl);

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            HtmlNode? a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            HtmlNode? img = a_tag?.SelectSingleNode(".//img");
            string imageLink = SiteText.ImageSource(img);
            string? title = img?.GetAttributeValue("alt", string.Empty);

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
