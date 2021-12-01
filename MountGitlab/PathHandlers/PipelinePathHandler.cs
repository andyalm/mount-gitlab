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

    protected override bool ExistsImpl()
    {
        if (!_parentHandler.Exists())
        {
            return false;
        }

        return GetItem() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        return GetPipeline();
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return Enumerable.Empty<GitlabObject>();
    }

    private GitlabPipeline? GetPipeline()
    {
        var pipeline = Context.GetGitlabObjects(p => new GitlabPipeline(ParentPath, p),
            p => _parentHandler.BranchName == null || p.Ref == _parentHandler.BranchName,
            "Get-GitlabPipeline",
            "-ProjectId", _parentHandler.ProjectPath,
            "-PipelineId", ItemName).FirstOrDefault();

        return pipeline;
    }
    
    private static readonly Regex PathRegex = new(@"pipelines/\d+$");
    public static bool Matches(string path)
    {
        return PathRegex.IsMatch(path);
    }
}