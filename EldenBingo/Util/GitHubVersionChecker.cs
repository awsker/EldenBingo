using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EldenBingo.Util
{
    public class GitHubVersionChecker
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/awsker/EldenBingo/releases";
        private readonly Version _currentVersion;
        private readonly HttpClient _httpClient;

        public GitHubVersionChecker(Version currentVersion)
        {
            _currentVersion = currentVersion;
            _httpClient = new HttpClient();
            // GitHub API requires a user agent
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("EldenBingo-VersionChecker", "1.0"));
        }

        public async Task<GitHubRelease?> CheckForNewerVersionAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
                var releases = JsonConvert.DeserializeObject<GitHubRelease[]>(response);

                if (releases == null || releases.Length == 0)
                    return null;

                var latestVersion = _currentVersion;
                GitHubRelease? latestRelease = null;
                string downloadUrl = string.Empty;
                bool hasNewer = false;

                foreach (var release in releases.OrderByDescending(r => new Version(r.Tag_Name)))
                {
                    // Skip pre-releases if needed
                    if (release.Prerelease)
                        continue;

                    var releaseVersion = new Version(release.Tag_Name);

                    if (releaseVersion > latestVersion)
                    {
                        hasNewer = true;
                        latestVersion = releaseVersion;
                        latestRelease = release;
                        // Get the .zip asset URL if available
                    }
                }
                return latestRelease;
            }
            catch (Exception ex)
            {
                MainForm.Instance?.PrintToConsole($"Error checking for new version: {ex.Message}", Color.Red, true);
                return null;
            }
        }

        public record struct GitHubRelease(string Name, string Tag_Name, string Html_Url, bool Prerelease, GitHubAsset[]? Assets);

        public record struct GitHubAsset(string Name, string Browser_Download_Url);
    }
}