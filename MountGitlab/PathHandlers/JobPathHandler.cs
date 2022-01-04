using System.Management.Automation.Provider;
using MountAnything;
using MountAnything.Content;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class JobPathHandler : PathHandler, IContentReaderHandler
{
    private readonly PipelinePathHandler _pipelineHandler;
    
    public JobPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch) : base(path, context)
    {
        _pipelineHandler = new PipelinePathHandler(ParentPath, Context, projectPath, currentBranch);
    }

    protected override bool ExistsImpl()
    {
        if (!_pipelineHandler.Exists())
        {
            return false;
        }

        return GetItem() != null;
    }

    protected override IItem? GetItemImpl()
    {
        var args = new List<string>
        {
            "-ProjectId", _pipelineHandler.ProjectPath.ToString()
        };
        if (long.TryParse(ItemName, out _))
        {
            args.AddRange(new[]
            {
                "-JobId", ItemName
            });
        }
        else
        {
            args.AddRange(new []
            {
                "-PipelineId", _pipelineHandler.PipelineId,
                "-Name", ItemName
            });
        }
        return Context.GetItems(j => new GitlabJob(ParentPath, j),
             job => job.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase) || job.Id.ToString() == ItemName,    
            "Get-GitlabJob", args.ToArray()).FirstOrDefault();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<IItem>();
    }

    public IContentReader GetContentReader()
    {
        return new JobContentReader((GitlabJob)GetItem()!, Context);
    }
}