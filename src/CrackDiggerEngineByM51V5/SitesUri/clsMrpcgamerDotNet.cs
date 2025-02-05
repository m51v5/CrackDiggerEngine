using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using CrackDiggerEngineByM51V5;
using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5.SitesUri
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

        public async Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode item)
        {
            // Get game Link
            var a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrEmpty(gameLink) && !gameLink!.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }

            // Get game image link from div data
            var img = a_tag.SelectSingleNode(".//img");
            string? imageLink = img?.GetAttributeValue("src", string.Empty);

            if (!string.IsNullOrEmpty(imageLink) && !imageLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                imageLink = img?.GetAttributeValue("data-src", string.Empty);
            }

            // Get game title
            string? title = a_tag?.GetAttributeValue("title", string.Empty);

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }

}