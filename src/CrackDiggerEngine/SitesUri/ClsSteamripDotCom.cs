using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class ClsSteamripDotCom : ISiteInfo
    {
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public static string siteUrl => "steamrip.com";
        public string searchParameter => "/?s=";
        public string mainNodeType => "div";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "post-element";

        // The search page is behind Cloudflare's bot check; the API isn't.
        public WordPressApi? wordPress => new(siteUrl);

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            HtmlNode? div = item.SelectSingleNode(".//div");
            string? imageLink = div?.GetAttributeValue("data-back-webp", string.Empty);
            if (string.IsNullOrEmpty(imageLink))
            {
                imageLink = div?.GetAttributeValue("data-back", string.Empty);
            }

            HtmlNode? a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            string? title = a_tag?.SelectSingleNode(".//span")?.InnerText;

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
