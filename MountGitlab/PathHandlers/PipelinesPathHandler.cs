using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinesPathHandler : PathHandler
{
    private static readonly Regex BranchPipeline = BranchPathHandler.BuildRegex("/pipelines");
    
    public PipelinesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        BranchPipelineMatch = BranchPipeline.Match(path);
    }
    
    private Match BranchPipelineMatch { get; }

    public string? BranchName => BranchPipelineMatch.Success ? BranchPipelineMatch.Groups["BranchName"].Value : null;

    public string ProjectPath => BranchPipelineMatch.Success ? BranchPipelineMatch.Groups["ProjectPath"].Value : ParentPath;

    protected override bool ExistsImpl()
    {
        return BranchPipelineMatch.Success
            ? new BranchPathHandler(ParentPath, Context).Exists()
            : new ProjectPathHandler(ProjectPath, Context).Exists();
    }

    protected override GitlabObject GetItemImpl()
    {
        return BranchPipelineMatch.Success ?
            new RefSection(ProjectPath, BranchName!, "pipelines") :
            new ProjectSection(ProjectPath, "pipelines");
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        var args = new List<string> { "-Project", ProjectPath };
        if (BranchPipelineMatch.Success)
        {
            args.Add("-Ref");
            args.Add(BranchName!);
        }
        var pipelines = Context.GetGitlabObjects(b => new GitlabPipeline(Path, b), "Get-GitlabPipeline", args.ToArray());
        foreach (var pipeline in pipelines)
        {
            Cache.SetItem(pipeline);

            yield return pipeline;
        }
    }
}