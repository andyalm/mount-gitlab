using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchesPathHandler : PathHandler
{
    public BranchesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public override bool Exists() => new ProjectPathHandler(ParentPath, Context).Exists();

    public override GitlabObject GetItem()
    {
        return new ProjectSection(ParentPath, "branches");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        var branches = Context.GetGitlabObjects(b => new GitlabBranch(ParentPath, b), "Get-GitlabBranch", "-Project", ParentPath);
        foreach (var branch in branches)
        {
            Cache.SetItem(branch);

            yield return branch;
        }
    }
}