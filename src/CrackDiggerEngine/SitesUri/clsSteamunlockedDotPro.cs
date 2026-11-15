using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    // Its search currently returns the home page; the engine detects that and reports it instead of unrelated results.
    internal class clsSteamunlockedDotPro : ISiteInfo
    {
        public static string siteUrl => "steamunlocked.pro";
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public string searchParameter => "/?s=";
        public string mainNodeType => "div";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "inside-article";

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            HtmlNode? div_tag = item.SelectSingleNode(".//div[contains(@class, 'post-image')]");
            if (div_tag == null)
            {
                return null;
            }
            HtmlNode? a_tag = div_tag.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            string imageLink = SiteText.ImageSource(a_tag?.SelectSingleNode(".//img"));
            string? title = item.SelectSingleNode(".//header")?.SelectSingleNode(".//h2")?.InnerText;

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
