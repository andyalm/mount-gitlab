using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using MountGitlab.PathHandlers;

namespace MountGitlab;
[CmdletProvider("MountGitlab", ProviderCapabilities.ExpandWildcards | ProviderCapabilities.Filter)]
public class MountGitlabProvider : NavigationCmdletProvider, IPathHandlerContext, IContentCmdletProvider
{
    private static readonly Cache _cache = new();

    public Cache Cache => _cache;

    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        return new Collection<PSDriveInfo>
        {
            new PSDriveInfo("gitlab", this.ProviderInfo, this.ItemSeparator.ToString(),
                "Allows you to navigate gitlab via your GitlabCli configuration", null)
        };
    }

    protected override string MakePath(string parent, string child)
    {
        var returnValue = base.MakePath(parent, child);
        // if (returnValue.StartsWith(Path.DirectorySeparatorChar) &&
        //     returnValue != Path.DirectorySeparatorChar.ToString())
        // {
        //     returnValue = returnValue.Substring(1);
        // }
        //WriteDebug($"{returnValue} MakePath({parent},{child})");

        return returnValue;
    }

    protected override string GetChildName(string path)
    {
        var returnValue = base.GetChildName(path);
        WriteDebug($"{returnValue} GetChildName({path})");

        return returnValue;
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers)
    {
        WriteDebug($"GetChildNames({path}, {returnContainers})");
        base.GetChildNames(path, returnContainers);
    }

    protected override void InvokeDefaultAction(string path)
    {
        WriteDebug($"InvokeDefaultAction({path})");
        base.InvokeDefaultAction(path);
    }

    protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
    {
        var returnValue = base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
        WriteDebug($"{returnValue} ConvertPath({path}, {filter}, {updatedPath}, {updatedFilter})");

        return returnValue;
    }

    protected override string GetParentPath(string path, string root)
    {
        var returnValue = base.GetParentPath(path, root);
        //WriteDebug($"{returnValue} GetParentPath({path}, {root})");

        return returnValue;
    }

    protected override string[] ExpandPath(string path)
    {
        WriteDebug($"ExpandPath({path})");
        var gitlabPath = GitlabPath.Normalize(path);
        var pathMatcher = new Regex("^" + Regex.Escape(gitlabPath).Replace(@"\*", ".*") + "$", RegexOptions.IgnoreCase);
        var parentHandler = GetPathHandler(GitlabPath.GetParent(gitlabPath));
        WriteDebug($"ExpandPath(pathMatcher: {pathMatcher}, parentPath: {GitlabPath.GetParent(gitlabPath)}, parentHandler: {parentHandler.GetType().Name}");
        try
        {
            var children = parentHandler.GetChildItems(recurse: false, useCache: true)
                .Where(i => pathMatcher.IsMatch(i.FullPath))
                .Select(i => ToProviderPath(i.FullPath))
                .Select(p => GitlabPath.GetParent(p) == "/" && p.StartsWith("/") ? p.Substring(1) : p)
                .ToArray();

            foreach (var child in children)
            {
                WriteDebug($"ExpandedPath: {child}");
            }

            return children;
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            throw;
        }
    }

    protected override bool IsValidPath(string path)
    {
        WriteDebug($"IsValidPath({path})");
        return true;
    }

    protected override bool ItemExists(string path)
    {
        if (path.Contains("*"))
        {
            return false;
        }
        
        var handler = GetPathHandler(path);
        var returnValue = handler.Exists();
        WriteDebug($"{returnValue} {handler.GetType().Name}.ItemExists({handler.Path})");

        return returnValue;
    }
    
    protected override void GetItem(string path)
    {
        WriteDebug($"GetItem({path})");
        var item = GetPathHandler(path).GetItem();
        if (item != null)
        {
            WriteGitlabObject(item);
        }
    }

    protected override bool HasChildItems(string path)
    {
        WriteDebug($"HasChildItems({path})");
        return GetPathHandler(path).GetChildItems(recurse:false, useCache:true).Any();
    }

    protected override bool IsItemContainer(string path)
    {
        WriteDebug($"IsItemContainer({path})");
        return GetPathHandler(path).GetItem()?.IsContainer ?? false;
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        var returnValue = base.NormalizeRelativePath(path, basePath);
        if (returnValue.StartsWith(Path.DirectorySeparatorChar) && basePath == Path.DirectorySeparatorChar.ToString())
        {
            returnValue = returnValue.Substring(1);
        }
        WriteDebug($"{returnValue} NormalizeRelativePath({path}, {basePath})");

        return returnValue;
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetChildItems(path, recurse);
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"GetChildItems({path}, {recurse})");
        try
        {
            var pathHandler = GetPathHandler(path);
            WriteDebug($"{pathHandler.GetType().Name}.GetChildItems({path})");
            var childItems = pathHandler.GetChildItems(recurse, useCache: false);
            childItems = pathHandler.NormalizeChildItems(childItems);
            WriteGitlabObjects(childItems);
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
        }
    }

    private void WriteGitlabObjects<T>(IEnumerable<T> gitlabObjects) where T : GitlabObject
    {
        foreach (var gitlabObject in gitlabObjects)
        {
            WriteGitlabObject(gitlabObject);
        }
    }
    
    private void WriteGitlabObject<T>(T gitlabObject) where T : GitlabObject
    {
        WriteDebug($"WriteItemObject<{gitlabObject.UnderlyingObject.TypeNames.First()}>(,{gitlabObject.FullPath},{gitlabObject.IsContainer})");
        var providerPath = ToProviderPath(gitlabObject.FullPath);
        WriteItemObject(gitlabObject.UnderlyingObject, providerPath, gitlabObject.IsContainer);
    }

    private static string ToProviderPath(string gitlabPath)
    {
        return $"{Path.DirectorySeparatorChar}{gitlabPath.Replace("/", Path.DirectorySeparatorChar.ToString())}";
        //return $"{gitlabPath.Replace("/", Path.DirectorySeparatorChar.ToString())}";
    }

    private IPathHandler GetPathHandler(string path)
    {
        try
        {
            path = GitlabPath.Normalize(path);
            if (path.StartsWith("/"))
            {
                throw new InvalidOperationException($"Path '{path}' is invalid. It should not start with a /");
            }

            if (string.IsNullOrEmpty(path))
            {
                return new RootPathHandler(path, this);
            }

            if (path.EndsWith("branches"))
            {
                return new BranchesPathHandler(path, this);
            }

            if (path.EndsWith("merge-requests"))
            {
                return new MergeRequestsPathHandler(path, this);
            }

            if (path.EndsWith("pipelines"))
            {
                return new PipelinesPathHandler(path, this);
            }

            if (MergeRequestPathHandler.Matches(path))
            {
                return new MergeRequestPathHandler(path, this);
            }

            if (JobPathHandler.Matches(path))
            {
                return new JobPathHandler(path, this);
            }

            if (PipelinePathHandler.Matches(path))
            {
                return new PipelinePathHandler(path, this);
            }
            
            if (BranchPathHandler.Matches(path))
            {
                return new BranchPathHandler(path, this);
            }

            if (FilesPathHandler.Matches(path))
            {
                return new FilesPathHandler(path, this);
            }
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
        }
        
        return new GroupOrProjectPathHandler(path, this);
    }

    public void ClearContent(string path)
    {
        throw new NotSupportedException();
    }

    public object ClearContentDynamicParameters(string path)
    {
        throw new NotSupportedException();
    }

    public IContentReader GetContentReader(string path)
    {
        var pathHandler = GetPathHandler(path);
        if (pathHandler is ISupportContentReader contentReadHandler)
        {
            return contentReadHandler.GetContentReader();
        }

        throw new InvalidOperationException("This item does not support reading content");
    }

    public object? GetContentReaderDynamicParameters(string path)
    {
        return null;
    }

    public IContentWriter GetContentWriter(string path)
    {
        throw new NotSupportedException();
    }

    public object GetContentWriterDynamicParameters(string path)
    {
        throw new NotSupportedException();
    }

    bool IPathHandlerContext.Force => base.Force.IsPresent;
}
