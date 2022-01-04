using System.Management.Automation.Provider;
using MountAnything;
using MountAnything.Content;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class FilesPathHandler : PathHandler, IContentReaderHandler
{
    private ItemPath FilePath { get; }

    private ItemPath FilesRootPath { get; }

    private ItemPath ProjectPath { get; }
    
    private CurrentBranch CurrentBranch { get; }
    
    public FilesPathHandler(ItemPath path, IPathHandlerContext context,
        ProjectPath projectPath, CurrentBranch currentBranch,
        FilePath filePath) : base(path, context)
    {
        ProjectPath = projectPath.ItemPath;
        CurrentBranch = currentBranch;
        FilesRootPath = Path.Ancestor("files");
        FilePath = filePath.ItemPath;
    }

    protected override bool ExistsImpl()
    {
        if (new GroupOrProjectPathHandler(ProjectPath, Context).GetProject() == null)
        {
            WriteDebug($"FilesPathHandler: Project {ProjectPath} does not exist");
            return false;
        }
    
        if (!CurrentBranch.IsDefault && !BranchesPathExists())
        {
            WriteDebug($"FilesPathHandler: Branch {CurrentBranch.Value} does not exist");
            return false;
        }
    
        return GetItem() != null;
    }

    protected override IItem? GetItemImpl()
    {
        WriteDebug($"FilesPathHandler.GetItemImpl(FilePath={FilePath}, FilesRootPath={FilesRootPath}, ProjectPath={ProjectPath})");
        if (FilePath.IsRoot)
        {
            return new GitlabRepositoryItem(FilesRootPath, FilePath, RepositoryItemType.Tree);
        }
        
        var file = GetFileAsTree();
        if (file != null)
        {
            return file;
        }

        var tree = GetTree();
        if (tree.Any())
        {
            return new GitlabRepositoryItem(FilesRootPath, FilePath,RepositoryItemType.Tree);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetTree();
    }

    private IEnumerable<GitlabRepositoryItem> GetTree()
    {
        return Context.GetItems(t => GitlabRepositoryItem.FromTree(FilesRootPath, t),
            "Get-GitlabRepositoryTree", WithOptionalRefParam("-Project", ProjectPath.ToString(), "-Path", FilePath.IsRoot ? "''" : FilePath.ToString()))
            .ToArray();
    }

    private GitlabRepositoryFile? GetFile()
    {
        return Context.GetPSObjects("Get-GitlabRepositoryFile", WithOptionalRefParam("-Project", ProjectPath.ToString(), "-FilePath", FilePath.ToString()))
            .Select(o => new GitlabRepositoryFile(o))
            .FirstOrDefault();
    }

    private GitlabRepositoryItem? GetFileAsTree()
    {
        return Context.GetItems(f => GitlabRepositoryItem.FromFile(FilesRootPath, f),
                "Get-GitlabRepositoryFile", WithOptionalRefParam("-Project", ProjectPath.ToString(), "-FilePath", FilePath.ToString()))
            .FirstOrDefault();
    }

    private bool BranchesPathExists()
    {
        return new BranchPathHandler(ProjectPath.Combine("branches", CurrentBranch.Value), Context)
            .Exists();
    }

    private string[] WithOptionalRefParam(params string[] args)
    {
        if (!CurrentBranch.IsDefault)
        {
            return args.Concat(new[] { "-Ref", CurrentBranch.Value }).ToArray();
        }

        return args;
    }

    public IContentReader GetContentReader()
    {
        var file = GetFile();
        if (file == null)
        {
            throw new InvalidOperationException($"File '{FilePath}' does not exist");
        }

        return new RepositoryFileReader(file);
    }
}