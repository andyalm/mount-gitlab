using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class MergeRequestsPathHandler : PathHandler
{
    public MergeRequestsPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl() => new ProjectPathHandler(ParentPath, Context).Exists();

    protected override GitlabObject GetItemImpl()
    {
        return new ProjectSection(ParentPath, "merge-requests");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGitlabObjects(b => new GitlabMergeRequest(b),
            "Get-GitlabMergeRequest", "-Project", ParentPath);
    }
}