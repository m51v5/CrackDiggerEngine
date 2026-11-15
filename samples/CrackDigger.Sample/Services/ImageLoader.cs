using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrackDigger.Sample.Services
{
    /// <summary>Downloads cover images with the sample's User-Agent, a few at a time, and caches them for the session.</summary>
    public static class ImageLoader
    {
        private static readonly HttpClient Http = CreateClient();
        private static readonly SemaphoreSlim Slots = new(4);
        private static readonly ConcurrentDictionary<string, Task<ImageSource?>> Cache = new();

        // Some image CDNs answer a missing cover with a 1×1 image.
        private const int MinSide = 16;

        private static HttpClient CreateClient()
        {
            HttpClient client = new() { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(AppLinks.UserAgent);
            return client;
        }

        /// <summary>The image, or null if it can't be loaded; never throws.</summary>
        public static Task<ImageSource?> LoadAsync(string url) => Cache.GetOrAdd(url, DownloadAsync);

        private static async Task<ImageSource?> DownloadAsync(string url)
        {
            if (!LinkOpener.IsWebLink(url))
            {
                return null;
            }

            await Slots.WaitAsync();
            try
            {
                byte[] bytes = await Http.GetByteArrayAsync(url);
                return await Task.Run(() => Decode(bytes));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or NotSupportedException or FileFormatException or InvalidOperationException or ArgumentException)
            {
                return null;
            }
            finally
            {
                Slots.Release();
            }
        }

        // Decoded small and frozen, so the UI thread can use it; tiny placeholders count as no image.
        private static ImageSource? Decode(byte[] bytes)
        {
            BitmapFrame frame = BitmapFrame.Create(new MemoryStream(bytes), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            if (frame.PixelWidth < MinSide || frame.PixelHeight < MinSide)
            {
                return null;
            }

            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelWidth = 400;
            image.StreamSource = new MemoryStream(bytes);
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
