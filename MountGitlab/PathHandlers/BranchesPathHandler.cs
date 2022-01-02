using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchesPathHandler : PathHandler
{
    public BranchesPathHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl() => new GroupOrProjectPathHandler(ParentPath, Context).GetProject() != null;

    protected override IItem GetItemImpl()
    {
        return new ProjectSection(ParentPath, "branches");
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Context.GetItems(b => new GitlabBranch(Path, b),
            "Get-GitlabBranch", "-Project", ParentPath.ToString());
    }
}