using System.Collections.ObjectModel;
using System.Management.Automation;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class ProjectPathHandler : PathHandler
{
    public ProjectPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return GetItemImpl() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        return Context.GetProjects("-ProjectId", Path).FirstOrDefault();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        yield return new ProjectSection(Path, "branches");
        yield return new ProjectSection(Path, "files");
        yield return new ProjectSection(Path, "merge-requests");
        yield return new ProjectSection(Path, "pipelines");
    }
}