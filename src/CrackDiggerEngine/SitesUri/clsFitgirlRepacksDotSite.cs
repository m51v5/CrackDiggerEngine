using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class clsFitgirlRepacksDotSite : ISiteInfo
    {
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public static string siteUrl => "fitgirl-repacks.site";
        public string searchParameter => "/?s=";
        public string mainNodeType => "h1";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "entry-title";

        // Posts have no featured image; the cover is the first image in the post.
        public WordPressApi? wordPress => new(siteUrl, ImageFromContent: true);

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            // The search page lists titles only, without images.
            HtmlNode? a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            string? title = a_tag?.InnerText;

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, string.Empty);
        }
    }
}
