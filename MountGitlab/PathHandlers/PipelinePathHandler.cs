using MountAnything;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinePathHandler : PathHandler
{
    private readonly PipelinesPathHandler _parentHandler;
    
    public PipelinePathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch) : base(path, context)
    {
        _parentHandler = new PipelinesPathHandler(ParentPath, Context, projectPath, currentBranch);
    }

    public ProjectPath ProjectPath => _parentHandler.ProjectPath;

    public string PipelineId => ItemName;

    protected override bool ExistsImpl()
    {
        if (!_parentHandler.Exists())
        {
            return false;
        }

        return GetItem() != null;
    }

    protected override IItem? GetItemImpl()
    {
        return GetPipeline();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Context.GetItems(j => new GitlabJob(Path, j),
            "Get-GitlabJob",
            "-ProjectId", _parentHandler.ProjectPath.ToString(),
            "-PipelineId", ItemName);
    }

    private GitlabPipeline? GetPipeline()
    {
        var pipeline = Context.GetItems(p => new GitlabPipeline(ParentPath, p),
            p => _parentHandler.CurrentBranch.IsDefault || p.Ref == _parentHandler.CurrentBranch.Value,
            "Get-GitlabPipeline",
            "-ProjectId", _parentHandler.ProjectPath.ToString(),
            "-PipelineId", ItemName).FirstOrDefault();

        return pipeline;
    }
}