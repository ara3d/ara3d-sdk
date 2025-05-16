using Ara3D.Utils;
using System.Net.Http.Headers;
using System.Text;
using Ara3D.Parsing.Json;
using Ara3D.Parsing.Markdown;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Cst.MarkdownInlineGrammarNameSpace;

namespace Ara3D.DevOpToolsAsTests
{
    public static class DevOpTools
    {
        public static IEnumerable<FilePath> GetAllProjects()
            => SourceCodeLocation.GetFolder().RelativeFolder("..", "..").GetFiles("*.csproj", true);
        
        public static HttpContent EncodeFormContent(params (string key, string value)[] formData) 
            => new FormUrlEncodedContent(formData.Select(tuple => new KeyValuePair<string, string>(tuple.key, tuple.value)));

        // https://stackoverflow.com/questions/5152723/curl-with-user-authentication-in-c-sharp
        // https://stackoverflow.com/questions/7929013/making-a-curl-call-in-c-sharp
        // https://stackoverflow.com/questions/21255725/webrequest-equivalent-to-curl-command
        public static async Task<string> PostWithAuthentication(string baseUrl, string path, string user, string pass, params (string key, string value)[] formData)
        {
            var authValue = new AuthenticationHeaderValue("Basic", 
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}")));
            try
            {
                var client = new HttpClient { DefaultRequestHeaders = { Authorization = authValue } };
                client.BaseAddress = new Uri(baseUrl);
                var content = EncodeFormContent(formData);
                var response = await client.PostAsync($"/{path}", content);
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "";
        }

        public static FilePath GithubApiKeyFile
            => @"C:\Users\cdigg\api-keys\github-public-api-token.txt";

        public static string GithubApiKey
            => GithubApiKeyFile.ReadAllText();

        // curl -d "text=This is a block of text" 
        // http://api.repustate.com/v2/demokey/score.json
        // curl -u YOUR_CLIENT_ID:YOUR_CLIENT_SECRET -I https://api.github.com/meta

        public static string[] Repos =
        {
            "ara3d/ara3d",
            "cdiggins/plato",
            "ara3d/parakeet",
        };
    }
}
