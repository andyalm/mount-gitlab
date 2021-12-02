namespace MountGitlab;

public static class GitlabPath
{
    public static string Normalize(string path)
    {
        var normalizedPath = path.Replace(@"\", "/");
        if (normalizedPath.StartsWith("/"))
        {
            return normalizedPath.Substring(1);
        }

        return normalizedPath;
    }
    
    public static string GetParent(string path)
    {
        return Path.GetDirectoryName(path)!.Replace(@"\", "/");
    }

    public static string Combine(params string[] parts)
    {
        return Path.Combine(parts).Replace(@"\", "/");
    }
}