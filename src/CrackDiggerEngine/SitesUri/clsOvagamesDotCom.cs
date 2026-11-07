using CrackDigger.SitesUri.Interfaces;
using CrackDigger;
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

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {            // Get game Link
            var a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrEmpty(gameLink) && gameLink?.StartsWith("http") == true && !gameLink.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }

            // Get game image link from div data
            var img = a_tag?.SelectSingleNode(".//img");
            string? imageLink = img?.GetAttributeValue("src", string.Empty);

            if (!string.IsNullOrEmpty(imageLink) && !imageLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                imageLink = img?.GetAttributeValue("data-src", string.Empty);
            }

            // Get game title
            string? title = img?.GetAttributeValue("alt", string.Empty)?.Trim();

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
