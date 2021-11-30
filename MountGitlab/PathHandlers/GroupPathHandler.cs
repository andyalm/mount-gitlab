using System.Collections.ObjectModel;
using System.Management.Automation;
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

        Collection<PSObject> response = null;
        WriteDebug($"Get-GitlabGroup -GroupId {Path}");
        try
        {
            response = InvokeCommand.InvokeScript($"Get-GitlabGroup -GroupId {Path}");
        }
        catch(CmdletInvocationException ex) when(ex.Message.Contains("404"))
        {
            WriteDebug(ex.ToString());
        }
        var rawGroup = response?.FirstOrDefault();
        if (rawGroup == null)
        {
            group = default!;
            return false;
        }
        
        WriteDebug($"GitlabGroup.Count: {response?.Count}");

        group = new GitlabGroup(rawGroup);
        Cache.SetItem(group);

        return true;
    }
}