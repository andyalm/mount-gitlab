using System.Text.RegularExpressions;
using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchPathHandler : PathHandler
{
    public BranchPathHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    public ItemPath ProjectPath => ParentPath.Parent;

    protected override IItem? GetItemImpl()
    {
        return GetBranch();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return new RefSection(ProjectPath, ItemName, "files");
        yield return new RefSection(ProjectPath, ItemName, "pipelines");
    }

    public GitlabBranch? GetBranch()
    {
        return Context.GetItems(b => new GitlabBranch(ParentPath, b), "Get-GitlabBranch", "-Project", ProjectPath.ToString(), "-Ref",
            ItemName).FirstOrDefault();
    }
}