using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class GroupPathHandler : PathHandler
{
    public GroupPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public override bool Exists()
    {
        return TryGetGroup(out _);
    }

    public override GitlabObject GetItem()
    {
        if (TryGetGroup(out var group))
        {
            return group;
        }

        throw new InvalidOperationException($"Group at path {Path} does not exist");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Context.GetGroups("-ParentGroupId", Path)
            .ToGitlabGroupOrProjects()
            .Concat(Context.GetProjects("-GroupId", Path).ToGitlabGroupOrProjects());
    }
    
    public bool TryGetGroup(out GitlabGroup group)
    {
        if (Cache.TryGetItem(Path, out GitlabGroup cachedGroup))
        {
            group = cachedGroup;
            return true;
        }
        
        WriteDebug($"Get-GitlabGroup -GroupId {Path}");
        var response = InvokeCommand.InvokeScript($"Get-GitlabGroup -GroupId {Path}");

        var rawGroup = response?.FirstOrDefault();
        if (rawGroup == null)
        {
            group = default!;
            return false;
        }

        group = new GitlabGroup(rawGroup);
        Cache.SetItem(group);

        return true;
    }
}