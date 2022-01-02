using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinesPathHandler : PathHandler
{
    public ProjectPath ProjectPath { get; }
    public CurrentBranch CurrentBranch { get; }
    
    public PipelinesPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch) : base(path, context)
    {
        ProjectPath = projectPath;
        CurrentBranch = currentBranch;
    }

    protected override bool ExistsImpl()
    {
        return CurrentBranch.IsDefault
            ? new GroupOrProjectPathHandler(ProjectPath.ItemPath, Context).GetProject() != null
            : new BranchPathHandler(ParentPath, Context).Exists();
    }

    protected override IItem GetItemImpl()
    {
        return CurrentBranch.IsDefault ?
            new ProjectSection(ProjectPath.ItemPath, "pipelines") :
            new RefSection(ProjectPath.ItemPath, CurrentBranch.Value, "pipelines");
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var args = new List<string> { "-Project", ProjectPath.ToString() };
        if (!CurrentBranch.IsDefault)
        {
            args.Add("-Ref");
            args.Add(CurrentBranch.Value);
        }
        return Context.GetItems(b => new GitlabPipeline(Path, b), "Get-GitlabPipeline", args.ToArray());
    }
}