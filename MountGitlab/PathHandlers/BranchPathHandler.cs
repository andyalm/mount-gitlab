using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchPathHandler : PathHandler
{
    public BranchPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public string ProjectPath => System.IO.Path.GetDirectoryName(ParentPath)!.Replace(@"\", "/");
    
    public override bool Exists()
    {
        if (Cache.TryGetItem(Path, out _))
        {
            return true;
        }

        return GetBranch() != null;
    }

    public override GitlabObject GetItem()
    {
        return GetBranch()!;
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }

    public Branch? GetBranch()
    {
        return Context.GetGitlabObjects(b => new Branch(ProjectPath, b), "Get-GitlabBranch", "-Project", ProjectPath, "-Ref",
            ItemName).FirstOrDefault();
    }

    private static readonly Regex PathRegex = new(@"branches/[a-z0-9_\-]+$", RegexOptions.IgnoreCase);
    public static bool Matches(string path)
    {
        return PathRegex.IsMatch(path);
    }
}