using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class MergeRequestPathHandler : PathHandler
{
    public MergeRequestPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath) : base(path, context)
    {
        ProjectPath = projectPath;
    }

    public ProjectPath ProjectPath { get; }

    protected override IItem? GetItemImpl()
    {
        return GetMergeRequest();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }

    public GitlabMergeRequest? GetMergeRequest()
    {
        return Context.GetItems(b => new GitlabMergeRequest(ParentPath, b),
            "Get-GitlabMergeRequest", "-Project", ProjectPath.ToString(), "-MergeRequestId", ItemName)
            .FirstOrDefault();
    }
}