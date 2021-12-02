﻿using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class PipelinePathHandler : PathHandler
{
    private readonly PipelinesPathHandler _parentHandler;
    
    public PipelinePathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _parentHandler = new PipelinesPathHandler(ParentPath, Context);
    }

    public string ProjectPath => _parentHandler.ProjectPath;

    public string PipelineId => ItemName;

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

    protected override IEnumerable<GitlabObject> GetChildItemsImpl(bool recurse)
    {
        return Context.GetGitlabObjects(j => new GitlabJob(Path, j),
            "Get-GitlabJob",
            "-ProjectId", _parentHandler.ProjectPath,
            "-PipelineId", ItemName);
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