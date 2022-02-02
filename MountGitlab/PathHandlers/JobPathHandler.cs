using System.Management.Automation.Provider;
using MountAnything;
using MountAnything.Content;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class JobPathHandler : PathHandler, IContentReaderHandler
{
    public IItemAncestor<GitlabPipeline> Pipeline { get; }
    private ProjectPath ProjectPath { get; }
    private CurrentBranch CurrentBranch { get; }
    
    public JobPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch, IItemAncestor<GitlabPipeline> pipeline) : base(path, context)
    {
        Pipeline = pipeline;
        ProjectPath = projectPath;
        CurrentBranch = currentBranch;
    }

    protected override IItem? GetItemImpl()
    {
        var args = new List<string>
        {
            "-ProjectId", ProjectPath.ToString()
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
                "-PipelineId", $"\"{Pipeline.Item.ItemName}\"",
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