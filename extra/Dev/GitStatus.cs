namespace Ara3D.Utils.Dev
{
    public class GitStatus
    {
        public string Hash { get; } = GitHelper.CommitHash();
        public string Branch { get; } = GitHelper.BranchName();
        public string Remote { get; } = GitHelper.Remote();
    }
}