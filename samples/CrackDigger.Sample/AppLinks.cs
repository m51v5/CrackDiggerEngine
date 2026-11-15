namespace CrackDigger.Sample
{
    /// <summary>Every address the sample opens or reads, in one place.</summary>
    public static class AppLinks
    {
        public const string Repository = "https://github.com/m51v5/CrackDiggerEngine";
        public const string RepositoryShort = "github.com/m51v5/CrackDiggerEngine";
        public const string Releases = Repository + "/releases";
        public const string Security = Repository + "/blob/main/SECURITY.md";
        public const string License = Repository + "/blob/main/LICENSE";
        public const string LatestReleaseApi = "https://api.github.com/repos/m51v5/CrackDiggerEngine/releases/latest";
        public const string Website = "https://m51v5.com";
        public const string Telegram = "https://t.me/m51v5";
        public const string Contact = "https://m51v5.com/contact";

        /// <summary>The library's version as major.minor.patch; the sample has no version of its own.</summary>
        public static string LibraryVersion { get; } = ReadLibraryVersion();

        /// <summary>The bug form with the library version already filled in.</summary>
        public static string ReportProblem => $"{Repository}/issues/new?template=bug_report.yml&version={LibraryVersion}";

        private static string ReadLibraryVersion()
        {
            Version v = typeof(CrackDiggerEngine).Assembly.GetName().Version ?? new Version(0, 0, 0);
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        /// <summary>User-Agent for the sample's own requests: images and the update check.</summary>
        public static string UserAgent => $"CrackDigger.Sample/{LibraryVersion} (+{Repository})";
    }
}
