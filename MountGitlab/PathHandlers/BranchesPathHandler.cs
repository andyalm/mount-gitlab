using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchesPathHandler : PathHandler
{
    public BranchesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl() => new ProjectPathHandler(ParentPath, Context).Exists();

    protected override GitlabObject GetItemImpl()
    {
        return new ProjectSection(ParentPath, "branches");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGitlabObjects(b => new GitlabBranch(ParentPath, b),
            "Get-GitlabBranch", "-Project", ParentPath);
    }
}