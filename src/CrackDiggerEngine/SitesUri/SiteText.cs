using HtmlAgilityPack;

namespace CrackDigger.SitesUri
{
    /// <summary>Cleans what parsers read from pages: entities, relative links, lazy images.</summary>
    internal static class SiteText
    {
        /// <summary>Decodes HTML entities such as &amp;#8211; and trims.</summary>
        public static string Clean(string? text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : HtmlEntity.DeEntitize(text).Trim();

        /// <summary>Makes a link absolute against baseUrl ("https://site"); empty stays empty.</summary>
        public static string Absolute(string? url, string baseUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            if (url.StartsWith("//", StringComparison.Ordinal))
            {
                return "https:" + url;
            }
            return baseUrl.TrimEnd('/') + "/" + url.TrimStart('/');
        }

        /// <summary>The result with its title, link and image cleaned and made absolute.</summary>
        public static CrackDiggerEngine.clsGameDataObject Normalize(CrackDiggerEngine.clsGameDataObject game, string baseUrl) =>
            new(Clean(game.Title), Absolute(Clean(game.GameLink), baseUrl), Absolute(Clean(game.ImageLink), baseUrl));

        /// <summary>An image's real address, preferring lazy-load attributes over a placeholder src.</summary>
        public static string ImageSource(HtmlNode? img)
        {
            if (img == null)
            {
                return string.Empty;
            }
            string src = img.GetAttributeValue("src", string.Empty);
            if (string.IsNullOrEmpty(src) || src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                src = img.GetAttributeValue("data-src", string.Empty);
            }
            if (string.IsNullOrEmpty(src))
            {
                src = img.GetAttributeValue("data-lazy-src", string.Empty);
            }
            return src;
        }

        /// <summary>The first image in an HTML fragment, or empty.</summary>
        public static string FirstImage(string? html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            return ImageSource(doc.DocumentNode.SelectSingleNode("//img"));
        }

        /// <summary>True when a WordPress site answered a search with its home page, so its search doesn't work.</summary>
        public static bool IsHomePage(HtmlDocument document)
        {
            string[] classes = document.DocumentNode.SelectSingleNode("//body")?.GetAttributeValue("class", string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
            return classes.Contains("home") && !classes.Contains("search");
        }
    }
}
