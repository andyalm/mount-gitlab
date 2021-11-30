using System.Collections.ObjectModel;
using System.Management.Automation;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class GroupPathHandler : PathHandler
{
    public GroupPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return GetItemImpl() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        return Context.GetGitlabObjects(g => new GitlabGroup(g),
                "Get-GitlabGroup", "-GroupId", Path)
            .FirstOrDefault();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGroups("-ParentGroupId", Path)
            .ToGitlabGroupOrProjects()
            .Concat(Context.GetProjects("-GroupId", Path).ToGitlabGroupOrProjects());
    }
}