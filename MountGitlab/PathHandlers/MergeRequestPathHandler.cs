using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class MergeRequestPathHandler : PathHandler
{
    public MergeRequestPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public string ProjectPath => GitlabPath.GetParent(ParentPath);

    protected override bool ExistsImpl()
    {
        return GetMergeRequest() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        return GetMergeRequest();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }

    public GitlabMergeRequest? GetMergeRequest()
    {
        return Context.GetGitlabObjects(b => new GitlabMergeRequest(b),
            "Get-GitlabMergeRequest", "-Project", ProjectPath, "-MergeRequestId", ItemName)
            .FirstOrDefault();
    }

    private static readonly Regex PathRegex = new(@"^(?<ProjectPath>.+)/merge-requests/(?<MergeRequestId>\d+)$");
    
    public static bool Matches(string path)
    {
        return PathRegex.IsMatch(path);
    }
}