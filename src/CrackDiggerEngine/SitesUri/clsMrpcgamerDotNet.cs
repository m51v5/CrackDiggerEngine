using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class clsMrpcgamerDotNet : ISiteInfo
    {
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public static string siteUrl => "mrpcgamer.net";
        public string searchParameter => "/?s=";
        public string mainNodeType => "div";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "postimge";

        // Posts have no featured image; the cover is the first image in the post.
        public WordPressApi? wordPress => new(siteUrl, ImageFromContent: true);

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            HtmlNode? a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            string imageLink = SiteText.ImageSource(a_tag?.SelectSingleNode(".//img"));
            string? title = a_tag?.GetAttributeValue("title", string.Empty);

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
