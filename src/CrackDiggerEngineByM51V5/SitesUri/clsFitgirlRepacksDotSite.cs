using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using CrackDiggerEngineByM51V5;
using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5.SitesUri
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

        public async Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode item)
        {
            // Get game image link from div data
            string imageLink = string.Empty;

            // Get game Link
            var a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrEmpty(gameLink) && !gameLink!.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }

            // Get game title
            string? title = a_tag?.InnerText.Trim();

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
