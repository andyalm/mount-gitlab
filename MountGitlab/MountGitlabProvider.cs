using System.Management.Automation.Provider;
using MountGitlab.Models;
using MountGitlab.PathHandlers;

namespace MountGitlab;
[CmdletProvider("MountGitlab", ProviderCapabilities.Filter)]
public class MountGitlabProvider : NavigationCmdletProvider, IPathHandlerContext
{
    private static readonly Cache _cache = new();

    public Cache Cache => _cache;

    protected override string MakePath(string parent, string child)
    {
        var returnValue = base.MakePath(parent, child);
        WriteDebug($"{returnValue} MakePath({parent},{child})");

        return returnValue;
    }

    protected override string GetParentPath(string path, string root)
    {
        var returnValue = base.GetParentPath(path, root);
        WriteDebug($"{returnValue} GetParentPath({path}, {root})");

        return returnValue;
    }

    protected override string[] ExpandPath(string path)
    {
        WriteDebug($"ExpandPath({path})");
        return base.ExpandPath(path);
    }

    protected override bool IsValidPath(string path)
    {
        WriteDebug($"IsValidPath({path})");
        return true;
    }

    protected override bool ItemExists(string path)
    {
        var handler = GetPathHandler(path);
        var returnValue = handler.Exists();
        WriteDebug($"{returnValue} {handler.GetType().Name}.ItemExists({handler.Path})");

        return returnValue;
    }
    
    protected override void GetItem(string path)
    {
        WriteDebug($"GetItem({path})");
        WriteGitlabObject(GetPathHandler(path).GetItem());
    }

    protected override bool HasChildItems(string path)
    {
        WriteDebug($"HasChildItems({path})");
        return base.HasChildItems(path);
    }

    protected override bool IsItemContainer(string path)
    {
        WriteDebug($"IsItemContainer({path})");
        return true;
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        var returnValue = base.NormalizeRelativePath(path, basePath);
        //WriteDebug($"{returnValue} NormalizeRelativePath({path}, {basePath})");

        return returnValue;
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        WriteDebug($"GetChildItems({path}, {recurse}, {depth})");
        GetChildItems(path, recurse);
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"GetChildItems({path}, {recurse})");

        WriteGitlabObjects(GetPathHandler(path).GetChildItems(recurse));
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
        var providerPath =
            $"{Path.DirectorySeparatorChar}{gitlabObject.FullPath.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString())}";
        WriteItemObject(gitlabObject.UnderlyingObject, providerPath, gitlabObject.IsContainer);
    }

    private IPathHandler GetPathHandler(string path)
    {
        try
        {
            path = ToNormalizedGitlabPath(path);
            if (path.StartsWith("/"))
            {
                throw new InvalidOperationException($"Path '{path}' is invalid. It should not start with a /");
            }

            if (string.IsNullOrEmpty(path))
            {
                return new RootPathHandler(path, this);
            }

            if (_cache.TryGetItem(path, out var cachedObject))
            {
                return cachedObject switch
                {
                    GitlabProject => new ProjectPathHandler(path, this),
                    GitlabGroup => new GroupPathHandler(path, this),
                    ProjectSection projectSection => GetProjectSectionHandler(projectSection, path),
                    GitlabBranch => new BranchPathHandler(path, this),
                    GitlabPipeline => new PipelinePathHandler(path, this),
                    RefSection refSection => GetRefSectionHandler(refSection, path),
                    _ => throw new ArgumentOutOfRangeException(nameof(cachedObject))
                };
            }

            if (path.EndsWith("branches"))
            {
                return new BranchesPathHandler(path, this);
            }

            if (path.EndsWith("pipelines"))
            {
                return new PipelinesPathHandler(path, this);
            }

            if (PipelinePathHandler.Matches(path))
            {
                return new PipelinePathHandler(path, this);
            }

            if (BranchPathHandler.Matches(path))
            {
                return new BranchPathHandler(path, this);
            }
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
        }
        
        return new GroupOrProjectPathHandler(path, this);
    }

    private IPathHandler GetRefSectionHandler(RefSection refSection, string path)
    {
        return refSection.Name switch
        {
            "pipelines" => new PipelinesPathHandler(path, this),
            _ => throw new ArgumentOutOfRangeException($"RefSection '{refSection.Name}' not currently supported")
        };
    }

    private IPathHandler GetProjectSectionHandler(ProjectSection projectSection, string path)
    {
        return projectSection.Name switch
        {
            "branches" => new BranchesPathHandler(path, this),
            _ => throw new ArgumentOutOfRangeException(
                $"ProjectSection '{projectSection.Name}' not currently supported")
        };
    }

    private string ToNormalizedGitlabPath(string path)
    {
        var normalizedPath = path.Replace(@"\", "/");
        if (normalizedPath.StartsWith("/"))
        {
            return normalizedPath.Substring(1);
        }
        else
        {
            return normalizedPath;
        }
    }
}
