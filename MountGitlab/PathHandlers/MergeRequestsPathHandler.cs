using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class MergeRequestsPathHandler : PathHandler
{
    public MergeRequestsPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath) : base(path, context)
    {
        ProjectPath = projectPath;
    }

    public ProjectPath ProjectPath { get; }

    protected override bool ExistsImpl() => new GroupOrProjectPathHandler(ProjectPath.ItemPath, Context).GetProject() != null;

    protected override IItem GetItemImpl()
    {
        return new ProjectSection(ProjectPath.ItemPath, "merge-requests");
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Context.GetItems(b => new GitlabMergeRequest(Path, b),
            "Get-GitlabMergeRequest", "-Project", ProjectPath.ToString());
    }
}