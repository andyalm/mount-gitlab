using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchPathHandler : PathHandler
{
    public BranchPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public string ProjectPath => GitlabPath.GetParent(ParentPath);

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        return GetBranch();
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        yield return new RefSection(ProjectPath, ItemName, "files");
        yield return new RefSection(ProjectPath, ItemName, "pipelines");
    }

    public GitlabBranch? GetBranch()
    {
        return Context.GetGitlabObjects(b => new GitlabBranch(ProjectPath, b), "Get-GitlabBranch", "-Project", ProjectPath, "-Ref",
            ItemName).FirstOrDefault();
    }

    private static readonly Regex PathRegex = BuildRegex("$");
    public static bool Matches(string path)
    {
        return PathRegex.IsMatch(path);
    }

    public static Regex BuildRegex(string suffix)
    {
        return new Regex(@$"^(?<ProjectPath>.+)/branches/(?<BranchName>[a-z0-9_\-]+){suffix}", RegexOptions.IgnoreCase);
    }
}