using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class ProjectPathHandler : PathHandler
{
    public ProjectPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public override bool Exists()
    {
        return TryGetProject(out _);
    }

    public override GitlabObject GetItem()
    {
        if (TryGetProject(out var project))
        {
            return project;
        }

        throw new InvalidOperationException($"Project at path {Path} does not exist");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }
    
    public bool TryGetProject(out GitlabProject project)
    {
        if (Cache.TryGetItem(Path, out GitlabProject cachedProject))
        {
            project = cachedProject;
            return true;
        }
        
        WriteDebug($"Get-GitlabProject -ProjectId {Path}");
        var response = InvokeCommand.InvokeScript($"Get-GitlabProject -ProjectId {Path}");

        var rawProject = response?.FirstOrDefault();
        if (rawProject == null)
        {
            project = default;
            return false;
        }

        project = new GitlabProject(rawProject);
        Cache.SetItem(project);

        return true;
    }
}