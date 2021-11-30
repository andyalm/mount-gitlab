using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class BranchesPathHandler : PathHandler
{
    public BranchesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    public override bool Exists() => true;

    public override GitlabObject GetItem()
    {
        return new ProjectSection(ParentPath, "branches");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        WriteDebug($"Get-GitlabBranch -Project {ParentPath}");
        var response = InvokeCommand.InvokeScript($"Get-GitlabBranch -Project {ParentPath}");
        if (response == null)
        {
            throw new InvalidOperationException($"Unexpected response from Get-GitlabBranch");
        }

        foreach (var rawBranch in response)
        {
            var branch = new Branch(ParentPath, rawBranch);
            Cache.SetItem(branch);

            yield return branch;
        }
    }
}