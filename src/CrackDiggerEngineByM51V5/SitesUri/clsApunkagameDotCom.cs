using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using CrackDiggerEngineByM51V5;
using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5.SitesUri
{
    internal class clsApunkagameDotCom : ISiteInfo
    {
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public static string siteUrl => "apunkagames.com";
        public string searchParameter => "/?s=";
        public string mainNodeType => "article";
        public string mainNodeDataName => "";
        public string mainNodeDataValue => "";

        public async Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode item)
        {
            // Get game image link from div data
            var img = item.SelectSingleNode(".//img");
            string? imageLink = img?.GetAttributeValue("src", string.Empty);

            if (!string.IsNullOrEmpty(imageLink) && !imageLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                imageLink = img?.GetAttributeValue("data-src", string.Empty);
            }

            // Get game Link
            var a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrEmpty(gameLink) && !gameLink!.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }

            // Get game title
            var h2_tag = item?.SelectSingleNode(".//h2")?.SelectSingleNode(".//a");
            string? title = h2_tag?.InnerText.Trim();

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
