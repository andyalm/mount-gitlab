using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class MergeRequestsPathHandler : PathHandler
{
    public MergeRequestsPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl() => new GroupOrProjectPathHandler(ParentPath, Context).GetProject() != null;

    protected override GitlabObject GetItemImpl()
    {
        return new ProjectSection(ParentPath, "merge-requests");
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        return Context.GetGitlabObjects(b => new GitlabMergeRequest(b),
            "Get-GitlabMergeRequest", "-Project", ParentPath);
    }
}