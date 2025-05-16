using System.Diagnostics;
using System.Net.Http.Headers;
using Ara3D.Logging;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Cst.MarkdownInlineGrammarNameSpace;
using Ara3D.Parsing.Json;
using Ara3D.Parsing.Markdown;
using Ara3D.Utils;

namespace Ara3D.DevOpToolsAsTests;

public static class AwesomeRepoTableGenerator
{
    public static DirectoryPath Folder => PathUtil.GetCallerSourceFolder();
    public static FilePath AwesomeDotNetMdFilePath => Folder.RelativeFile("awesome-dotnet.md");
    public static FilePath AwesomeUrls => Folder.RelativeFile("awesome-urls.txt");
    public static FilePath ExtraAwesomeMd => Folder.RelativeFile("extra-awesome-dotnet.md");
    public static FilePath RepoJson => Folder.RelativeFile("repos.json");
    public static FilePath EmojiJson => Folder.RelativeFile("github-emojis.json");

    // TODO: change this for your API key.
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

    // https://gist.github.com/MaximRouiller/74ae40aa994579393f52747e78f26441
    public static async Task<string> GithubApiQuery(string path, string token)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.github.com");
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
        var response = await client.GetAsync($"/{path}");
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }

    [TestCaseSource(nameof(Repos))]
    public static void TestQueryGithub(string repoName)
    {
        var response = GithubApiQuery($"repos/{repoName}", GithubApiKey).Result;
        Console.WriteLine(response);
        var parser = new JsonParser(response);
        var xml = parser.Parser.ParseXml;
        Console.WriteLine(xml);
        var prettifiedJson = parser.Root.BuildString().ToString();
        Console.WriteLine(prettifiedJson);
    }
    
    public static JsonElement GithubApiQueryJson(string repoName)
        => GithubApiQuery($"repos/{repoName}", GithubApiKey).Result.ParseJson();

    [Test, Explicit]
    public static void OutputUrlsToFile()
    {
        var logger = Logger.Debug;
        var urls = GetAllLinkedUrls(AwesomeDotNetMdFilePath, logger).ToList();
        AwesomeUrls.WriteAllLines(urls);
    }

    [Test, Explicit]
    public static void GenerateRepoJson()
    {
        var lines = AwesomeUrls.ReadAllLines();
        var repos = new List<JsonObject>();
        foreach (var line in lines)
        {
            var (cat, url) = line.SplitAtChar('=');
            var repoData = GetRepoData(url, cat);
            if (repoData == null)
                continue;
            var obj = JsonObject.Create(repoData);
            repos.Add(obj);
        }

        var array = JsonArray.Create(repos);
        var elementData = array.Elements.JoinStrings(",\n");
        Console.WriteLine(elementData);
        RepoJson.WriteAllText($"[\n{elementData}\n]\n");
    }

    [Test, Explicit]
    public static void GenerateGithubEmojiCode()
    {
        var input = EmojiJson.ReadAllJson();
        var j = input as JsonObject;
        var lines = EmojiJson
            .RelativeFile("emojis.txt")
            .ReadAllLines()
            .Where(line => !line.IsNullOrWhiteSpace())
            .ToList();
        Debug.Assert(lines.Count == j.Count);

        for (var i = 0; i < lines.Count; ++i)
        {
            var f = j.Fields[i];
            var l = lines[i];
            Console.WriteLine($"{{ \"{f.Key.AsString}\", \"{l}\" }},");
        }
    }

    [Test, Explicit]
    public static void GenerateEmojiList()
    {
        var input = EmojiJson.ReadAllJson();
        var j = input as JsonObject;
        foreach (var field in j.Fields)
        {
            Console.WriteLine($":{field.Key.AsString}:");
            Console.WriteLine();
        }
    }

    [Test, Explicit]
    public static void GenerateMarkdown()
    {
        var hb = new HtmlBuilder();
        var repoText = RepoJson.ReadAllText();
        var p = new JsonParser(repoText, Logger.Debug);
        Verifier.Assert(p.Parser.Succeeded);
        if (p.Root is JsonArray ja)
        {
            var repos = ja.Elements.Select(e => e.Convert<RepoData>()).ToList();
            WriteRepoTables(hb, repos);
            var html = hb.ToString();
            Console.WriteLine(html);
            ExtraAwesomeMd.WriteAllText(html);
        }
    }

    public static IEnumerable<string> GetAllLinkedUrls(FilePath filePath, ILogger logger)
    {
        var markdown = filePath.ReadAllText();
        var pBlock = new MarkdownBlockParser(markdown, logger);
        if (!pBlock.Parser.Succeeded) throw new Exception("Failed to parse markdown");
        logger.Log("Parsed markdown block");

        var category = "Unknown";
        foreach (var c in pBlock.Document.Children)
        {
            if (c is MdHeader h)
            {
                category = h.Content.Text;
                Console.WriteLine($"Category changed to {category}");
            }
            else
            {
                var blocks = c.GetAllTextBlocks().ToList();
                logger.Log("Retrieved blocks");
                foreach (var tb in blocks)
                {
                    var pInline = tb.ParseInlineMarkdown(logger);

                    foreach (var url in pInline.Parser.Cst.Descendants().OfType<CstUrl>())
                        yield return $"{category}={url.Text}";
                }
            }
        }
    }

    public static string GetRepoName(string repoUrl)
    {
        var prefix = "https://github.com/";
        if (repoUrl?.StartsWith(prefix) == true)
        {
            return repoUrl.Substring(prefix.Length);
        }
        return null;
    }


    public static RepoData GetRepoData(JsonElement j, string category)
    {
        var r = new RepoData
        {
            Url = j["html_url"],
            ShortName = j["name"],
            FullName = j["full_name"],
            Owner = j["owner"]["login"],
            Created = j["created_at"].AsString.SplitAtChar('T').Item1,
            Updated = j["updated_at"].AsString.SplitAtChar('T').Item1,
            Stars = j["watchers"],
            Forks = j["forks_count"],
            Issues = j["open_issues_count"],
            License = j["license"]["name"],
            Description = j["description"],
            Category = category,
        };
        return r;
    }

    public static RepoData GetRepoData(string repoUrl, string category)
    {
        var name = GetRepoName(repoUrl);
        if (name == null) return null;

        try
        {
            var json = GithubApiQueryJson(name);
            return GetRepoData(json, category);
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex);
            return null;
        }
    }
    
    public static HtmlBuilder Write(this HtmlBuilder hb, RepoData repo)
    {
        if (repo == null) return hb;
        hb.WriteStartTag("tr").WriteLine();
        hb.WriteTaggedHyperlink("td", repo.Url, repo.ShortName);
        hb.WriteTaggedHyperlink("td", repo.OwnerUrl, repo.Owner);
        //hb.WriteTaggedText("td", repo.Created);
        //hb.WriteTaggedText("td", repo.Updated);
        hb.WriteTaggedText("td", $"{repo.Stars} \u2b50");
        hb.WriteTaggedText("td", $"{repo.Forks} \ud83c\udf74");
        hb.WriteTaggedText("td", $"{repo.Issues} \ud83d\udd28");
        hb.WriteTaggedText("td", repo.License);
        hb.WriteEndTag("tr").WriteLine();
        hb.WriteStartTag("tr");
        hb.WriteStartTag("td", ("colspan", "8"));
        hb.WriteLine($"&nbsp;&nbsp;&nbsp;&nbsp;");
        if (repo.Description != "null" && repo.Description != "undefined")
            hb.WriteEscaped(repo.Description.ReplaceEmojis());
        hb.WriteEndTag("tr").WriteLine();
        return hb;
    }


    public static HtmlBuilder WriteRepoTables(this HtmlBuilder hb, IEnumerable<RepoData> repos)
    {
        var d = repos.ToDictionaryOfLists(r => r.Category, r => r);
        var orderedKeys = d.Keys.OrderBy(key => key).ToList();

        hb.WriteLine("# Table of Contents");
        hb.WriteLine();
        foreach (var key in orderedKeys)
        {
            var a = key.ToHtmlAnchor();
            hb.WriteLine($"* [{key}](#{a})");
        }

        foreach (var key in orderedKeys)
        {
            var g = d[key].OrderByDescending(r => r.Stars).ToList();
            hb.WriteLine($"## {key}");
            hb.WriteLine();
            hb.WriteRepoTable(g);
            hb.WriteLine();
        }

        return hb;
    }

    public static HtmlBuilder WriteRepoTable(this HtmlBuilder hb, IEnumerable<RepoData> repos)
    {
        hb.WriteStartTag("table");
        hb.WriteStartTag("thead");
        hb.WriteEndTag("tr").WriteLine();
        hb.WriteTaggedText("th", "Name");
        hb.WriteTaggedText("th", "Owner");
        hb.WriteTaggedText("th", "Stars");
        hb.WriteTaggedText("th", "Forks");
        hb.WriteTaggedText("th", "Issues");
        hb.WriteTaggedText("th", "License");
        hb.WriteEndTag("tr").WriteLine();
        hb.WriteEndTag("thead");
        hb.WriteStartTag("tbody");
        foreach (var repo in repos)
            hb = hb.Write(repo);
        hb.WriteEndTag("tbody");
        hb.WriteEndTag("table").WriteLine();
        return hb;
    }

}