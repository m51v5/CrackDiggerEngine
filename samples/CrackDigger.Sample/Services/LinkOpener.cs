using System.Diagnostics;
using System.Windows;

namespace CrackDigger.Sample.Services
{
    /// <summary>Opens links in the default browser and copies them.</summary>
    public static class LinkOpener
    {
        public static void Open(string? url)
        {
            if (IsWebLink(url))
            {
                Process.Start(new ProcessStartInfo(url!) { UseShellExecute = true });
            }
        }

        public static void Copy(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            try
            {
                Clipboard.SetText(url);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // Another app holds the clipboard; the user can try again.
            }
        }

        // Only http(s) links reach the shell, so a site can't make the sample start a program.
        public static bool IsWebLink(string? url) =>
            Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}
