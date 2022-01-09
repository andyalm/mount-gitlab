using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinePathHandler : PathHandler
{
    public ProjectPath ProjectPath { get; }
    public CurrentBranch CurrentBranch { get; }
    
    public PipelinePathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch) : base(path, context)
    {
        ProjectPath = projectPath;
        CurrentBranch = currentBranch;
    }

    public string PipelineId => ItemName;

    protected override IItem? GetItemImpl()
    {
        return GetPipeline();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Context.GetItems(j => new GitlabJob(Path, j),
            "Get-GitlabJob",
            "-ProjectId", ProjectPath.ToString(),
            "-PipelineId", ItemName);
    }

    private GitlabPipeline? GetPipeline()
    {
        var pipeline = Context.GetItems(p => new GitlabPipeline(ParentPath, p),
            p => CurrentBranch.IsDefault || p.Ref == CurrentBranch.Value,
            "Get-GitlabPipeline",
            "-ProjectId", ProjectPath.ToString(),
            "-PipelineId", ItemName).FirstOrDefault();

        return pipeline;
    }
}