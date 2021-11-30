using System.Collections.ObjectModel;
using System.Management.Automation;
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
        yield return new ProjectSection(Path, "branches");
        yield return new ProjectSection(Path, "pipelines");
    }
    
    public bool TryGetProject(out GitlabProject project)
    {
        if (Cache.TryGetItem(Path, out GitlabProject cachedProject))
        {
            project = cachedProject;
            return true;
        }

        var projects = Context.GetProjects("-ProjectId", Path);
        if (projects.Any())
        {
            project = projects.First();
            return true;
        }
        
        project = default!;
        return false;
    }
}