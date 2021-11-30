using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinePathHandler : PathHandler
{
    private readonly PipelinesPathHandler _parentHandler;
    
    public PipelinePathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _parentHandler = new PipelinesPathHandler(ParentPath, Context);
    }

    public override bool Exists()
    {
        if (Cache.TryGetItem(Path, out _))
        {
            return true;
        }
        
        if (!_parentHandler.Exists())
        {
            return false;
        }

        return GetPipeline() != null;
    }

    public override GitlabObject GetItem()
    {
        return GetPipeline()!;
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }

    private GitlabPipeline? GetPipeline()
    {
        var pipeline = Context.GetGitlabObjects(p => new GitlabPipeline(ParentPath, p),
            "Get-GitlabPipeline",
            "-ProjectId", _parentHandler.ProjectPath,
            "-PipelineId", ItemName).FirstOrDefault();

        if (pipeline != null && _parentHandler.BranchName != null && pipeline.Ref != _parentHandler.BranchName)
        {
            return null;
        }

        return pipeline;
    }
    
    private static readonly Regex PathRegex = new(@"pipelines/\d+$");
    public static bool Matches(string path)
    {
        return PathRegex.IsMatch(path);
    }
}