using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AutoGimkit
{
    internal static class UpdateChecker
    {
        public record GithubRelease([property: JsonPropertyName("tag_name")] string TagName);

        private const string RELEASE_URL = "https://api.github.com/repos/Destruction-Studios/AutoGimkit-Rewrite/releases/latest";
        private static HttpClient httpClient = new HttpClient();

        private static (bool Success, string? AppVersion, string? LatestVersion) CheckVersionAgainstCurrent(string latestVersion)
        {
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
            if (appVersion == null)
                return (false, null, null);

            string appVersionParsed = appVersion.Replace(".", "");
            appVersionParsed = appVersionParsed.Substring(0, latestVersion.Length - 2);
            string latestVersionParsed = latestVersion.Replace(".", "");

            bool s1 = int.TryParse(appVersionParsed, out int appInt);
            bool s2 = int.TryParse(latestVersionParsed, out int latestInt);

            if (s1 == false || s2 == false)
            {
                Debug.WriteLine($"Failed to parse s1: {s1} s2: {s2}");
                return (false, null, null);
            }

            Debug.WriteLine($"Current: {appInt} | Latest: {latestInt}");



            return (latestInt > appInt, appVersion, latestVersion);
        }

        public static async Task<(bool, string?, string?)> CheckForUpdate()
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CSharp-HttpClient-App");

            try
            {
                var release = await httpClient.GetFromJsonAsync<GithubRelease>(RELEASE_URL);

                if (release != null)
                {
                    //Debug.WriteLine($"Release: {release.TagName}");
                    return CheckVersionAgainstCurrent(release.TagName);
                }

            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data: {ex.Message}");
            }

            return (false, null, null);
        }
    }
}
