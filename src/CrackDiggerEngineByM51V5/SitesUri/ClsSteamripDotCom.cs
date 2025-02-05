using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using CrackDiggerEngineByM51V5;
using HtmlAgilityPack;

namespace CrackDiggerEngineByM51V5.SitesUri
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

        public async Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode item)
        {
            // Get game image link from div data
            var div = item.SelectSingleNode(".//div");
            string? imageLink = div?.GetAttributeValue("data-back-webp", string.Empty);
            if (string.IsNullOrEmpty(imageLink))
            {
                imageLink = div?.GetAttributeValue("data-back", string.Empty);
            }

            // Get game Link
            var a_tag = item.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);

            if (!gameLink.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }

            // Get game title
            var span_tag = a_tag?.SelectSingleNode(".//span");
            string? title = span_tag?.InnerText.Trim();

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
