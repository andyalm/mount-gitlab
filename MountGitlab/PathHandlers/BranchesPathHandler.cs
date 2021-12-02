using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchesPathHandler : PathHandler
{
    public BranchesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl() => new GroupOrProjectPathHandler(ParentPath, Context).GetProject() != null;

    protected override GitlabObject GetItemImpl()
    {
        return new ProjectSection(ParentPath, "branches");
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        return Context.GetGitlabObjects(b => new GitlabBranch(ParentPath, b),
            "Get-GitlabBranch", "-Project", ParentPath);
    }
}