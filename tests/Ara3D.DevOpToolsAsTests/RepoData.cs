namespace Ara3D.DevOpToolsAsTests;

public class RepoData
{
    public string ShortName;
    public string FullName;
    public string Url;
    public string Owner;
    public string Created;
    public string Updated;
    public string License;
    public int Stars;
    public int Forks;
    public int Issues;
    public string Description;
    public string Category;
    public string OwnerUrl => $"https://github.com/{Owner}";
}