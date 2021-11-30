namespace MountGitlab;

public static class GitlabPath
{
    public static string GetParent(string path)
    {
        return Path.GetDirectoryName(path)!.Replace(@"\", "/");
    }
}