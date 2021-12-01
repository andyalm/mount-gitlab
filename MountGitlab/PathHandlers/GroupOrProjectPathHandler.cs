﻿using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class GroupOrProjectPathHandler : PathHandler
{
    private readonly GroupPathHandler _groupHandler;
    private readonly ProjectPathHandler _projectHandler;
    
    public GroupOrProjectPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _groupHandler = new GroupPathHandler(path, context);
        _projectHandler = new ProjectPathHandler(path, context);
    }

    protected override bool ExistsImpl()
    {
        return GetItemImpl() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        if (_groupHandler.Exists())
        {
            return _groupHandler.GetItem();
        }
        if(_projectHandler.Exists())
        {
            return _projectHandler.GetItem();
        }

        return null;
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        var item = GetItem();
        return item switch
        {
            GitlabProject => _projectHandler.GetChildItems(recurse),
            GitlabGroup => _groupHandler.GetChildItems(recurse),
            _ => Enumerable.Empty<GitlabObject>()
        };
    }
}