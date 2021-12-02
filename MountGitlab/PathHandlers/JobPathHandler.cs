using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class JobPathHandler : PathHandler, ISupportContentReader
{
    private static readonly Regex _jobPathRegex = new(@"/pipelines/\d+/[^/]+$");
    public static bool Matches(string path)
    {
        return _jobPathRegex.IsMatch(path);
    }
    
    private readonly PipelinePathHandler _pipelineHandler;
    
    public JobPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _pipelineHandler = new PipelinePathHandler(ParentPath, Context);
    }

    protected override bool ExistsImpl()
    {
        if (!_pipelineHandler.Exists())
        {
            return false;
        }

        return GetItem() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        var args = new List<string>
        {
            "-ProjectId", _pipelineHandler.ProjectPath
        };
        var itemPropertyName = "Name";
        if (long.TryParse(ItemName, out _))
        {
            args.AddRange(new[]
            {
                "-JobId", ItemName
            });
            itemPropertyName = "Id";
        }
        else
        {
            args.AddRange(new []
            {
                "-PipelineId", _pipelineHandler.PipelineId,
                "-Name", ItemName
            });
        }
        return Context.GetGitlabObjects(j => new GitlabJob(ParentPath, j, itemPropertyName),
             job => job.JobName.Equals(ItemName, StringComparison.OrdinalIgnoreCase) || job.Id.ToString() == ItemName,    
            "Get-GitlabJob", args.ToArray()).FirstOrDefault();
    }

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }

    public IContentReader GetContentReader()
    {
        return new JobContentReader((GitlabJob)GetItem()!, Context);
    }
}