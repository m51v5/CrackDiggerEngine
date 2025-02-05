using CrackDiggerEngineByM51V5.SitesUri.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackDiggerEngineByM51V5.SitesUri
{
    internal class clsSteamunlockedDotPro :ISiteInfo
    {
        public static string siteUrl => "steamunlocked.pro";
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public string searchParameter => "/?s=";
        public string mainNodeType => "div";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "inside-article";

        public async Task<CrackDiggerEngine.clsGameDataObject> GetSingleGameDataAsync(HtmlNode item)
        {
            // Get game image link from div data
            var div_tag = item?.SelectSingleNode(".//div[contains(@class, 'post-image')]");
            if (div_tag == null)
            {
                return null;
            }
            var a_tag = div_tag.SelectSingleNode(".//a");
            string? gameLink = a_tag?.GetAttributeValue("href", string.Empty);
            if (!string.IsNullOrEmpty(gameLink) && gameLink.StartsWith("http") && !gameLink!.Contains(siteUri))
            {
                gameLink = $"{protocol}{siteUri}/{gameLink}";
            }


            var img_tag = a_tag?.SelectSingleNode(".//img");
            string? imageLink = img_tag?.GetAttributeValue("src", string.Empty);

            // Get game Link
            var header_tag = item?.SelectSingleNode(".//header");
            var h2_tag = header_tag?.SelectSingleNode(".//h2");

            // Get game title
            string? title = h2_tag?.InnerText.Trim();

            // Save data
            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, imageLink);
        }
    }
}
