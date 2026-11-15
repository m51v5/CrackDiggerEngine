using System.Text.Json;

namespace CrackDigger.SitesUri
{
    /// <summary>A site's WordPress REST API: its host, and whether covers must come from the post body.</summary>
    internal sealed record WordPressApi(string Host, bool ImageFromContent = false);

    /// <summary>Searches post titles through the WordPress REST API, which doesn't change with the site's theme.</summary>
    internal static class WordPressSearch
    {
        public static string SearchUrl(WordPressApi api, string keyword)
        {
            string fields = "title,link,yoast_head_json.og_image,_links,_embedded" + (api.ImageFromContent ? ",content" : "");
            return $"https://{api.Host}/wp-json/wp/v2/posts?search={Uri.EscapeDataString(keyword)}"
                + "&search_columns=post_title&orderby=relevance&per_page=20&_embed=wp:featuredmedia"
                + $"&_fields={fields}";
        }

        /// <summary>The results; throws when the API is off or answers with something else.</summary>
        public static async Task<List<CrackDiggerEngine.clsGameDataObject>> SearchAsync(HttpClient http, WordPressApi api, string keyword)
        {
            using HttpResponseMessage response = await http.GetAsync(SearchUrl(api, keyword));
            response.EnsureSuccessStatusCode();
            using JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            string baseUrl = "https://" + api.Host;
            List<CrackDiggerEngine.clsGameDataObject> data = new();
            foreach (JsonElement post in json.RootElement.EnumerateArray())
            {
                string title = Rendered(post, "title");
                string link = post.TryGetProperty("link", out JsonElement l) ? l.GetString() ?? string.Empty : string.Empty;
                string image = FeaturedImage(post);
                if (image.Length == 0)
                {
                    image = OgImage(post);
                }
                if (image.Length == 0 && api.ImageFromContent)
                {
                    image = SiteText.FirstImage(Rendered(post, "content"));
                }
                data.Add(SiteText.Normalize(new CrackDiggerEngine.clsGameDataObject(title, link, image), baseUrl));
            }
            return data;
        }

        private static string Rendered(JsonElement post, string name) =>
            post.TryGetProperty(name, out JsonElement field) && field.ValueKind == JsonValueKind.Object
                && field.TryGetProperty("rendered", out JsonElement text)
                ? text.GetString() ?? string.Empty
                : string.Empty;

        private static string FeaturedImage(JsonElement post)
        {
            if (post.TryGetProperty("_embedded", out JsonElement embedded)
                && embedded.TryGetProperty("wp:featuredmedia", out JsonElement media)
                && media.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in media.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("source_url", out JsonElement url))
                    {
                        return url.GetString() ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }

        private static string OgImage(JsonElement post)
        {
            if (post.TryGetProperty("yoast_head_json", out JsonElement yoast)
                && yoast.ValueKind == JsonValueKind.Object
                && yoast.TryGetProperty("og_image", out JsonElement images)
                && images.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in images.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("url", out JsonElement url))
                    {
                        return url.GetString() ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }
    }
}
