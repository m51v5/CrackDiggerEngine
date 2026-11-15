using CrackDigger.SitesUri.Interfaces;
using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    internal class clsCrackedGamesDotOrg : ISiteInfo
    {
        public string protocol => "https://";
        public string siteUri => siteUrl;
        public static string siteUrl => "cracked-games.org";

        // Search address from the site's schema.org SearchAction; /?s= now shows the home page.
        public string searchParameter => "/search?q=";

        // Each result is a cover link: <a class="block relative aspect-[3/4] ..."><img alt="Title"></a>
        public string mainNodeType => "a";
        public string mainNodeDataName => "class";
        public string mainNodeDataValue => "block relative aspect-[3/4]";

        public CrackDiggerEngine.clsGameDataObject? GetSingleGameData(HtmlNode item)
        {
            HtmlNode? img = item.SelectSingleNode(".//img");
            if (img == null)
            {
                return null;
            }
            string? gameLink = item.GetAttributeValue("href", string.Empty);
            string? title = img.GetAttributeValue("alt", string.Empty);

            return new CrackDiggerEngine.clsGameDataObject(title, gameLink, SiteText.ImageSource(img));
        }
    }
}
