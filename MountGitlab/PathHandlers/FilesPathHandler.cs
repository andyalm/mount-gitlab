using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using MountGitlab.Models;

namespace MountGitlab.PathHandlers;

public class FilesPathHandler : PathHandler, ISupportContentReader
{
    private static readonly Regex BranchesPathRegex = BranchPathHandler.BuildRegex("/files/?(?<FilePath>.*)$");
    private static readonly Regex ProjectPathRegex = new(@"^(?<ProjectPath>.+)/files/?(?<FilePath>.*)$");

    public static bool Matches(string path)
    {
        return BranchesPathRegex.IsMatch(path) || ProjectPathRegex.IsMatch(path);
    }

    private readonly Match _branchesPathMatch;
    public string ProjectPath { get; }

    public string FilePath { get; }
    
    public string FilesRootPath { get; }
    
    public FilesPathHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _branchesPathMatch = BranchesPathRegex.Match(path);
        if (_branchesPathMatch.Success)
        {
            ProjectPath = _branchesPathMatch.Groups["ProjectPath"].Value;
            FilePath = _branchesPathMatch.Groups["FilePath"].Value;
            FilesRootPath = GitlabPath.Combine(ProjectPath, "branches", _branchesPathMatch.Groups["BranchName"].Value, "files");
        }
        else
        {
            var projectPathMatch = ProjectPathRegex.Match(path);
            ProjectPath = projectPathMatch.Groups["ProjectPath"].Value;
            FilePath = projectPathMatch.Groups["FilePath"].Value;
            FilesRootPath = GitlabPath.Combine(ProjectPath, "files");
        }
    }

    protected override bool ExistsImpl()
    {
        if (!new ProjectPathHandler(ProjectPath, Context).Exists())
        {
            WriteDebug($"FilesPathHandler: Project {ProjectPath} does not exist");
            return false;
        }

        if (_branchesPathMatch.Success && !BranchesPathExists())
        {
            WriteDebug($"FilesPathHandler: Branch {_branchesPathMatch.Groups["BranchName"].Value} does not exist");
            return false;
        }

        return GetItem() != null;
    }

    protected override GitlabObject? GetItemImpl()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            return new GitlabRepositoryTree(FilesRootPath, new PSObject(new
            {
                Name = "files",
                Type = "tree",
                Path
            }));
        }
        
        var file = GetFile();
        if (file != null)
        {
            return file;
        }

        var tree = GetTree();
        if (tree.Any())
        {
            return new GitlabRepositoryTree(FilesRootPath, new PSObject(new
            {
                Name = ItemName,
                Type = "tree",
                Path = FilePath
            }));
        }

        return null;
    }

    public override IEnumerable<GitlabObject> GetChildItems(bool recurse)
    {
        return GetTree();
    }

    private IEnumerable<GitlabRepositoryTree> GetTree()
    {
        return Context.GetGitlabObjects(t => new GitlabRepositoryTree(FilesRootPath, t),
            "Get-GitlabRepositoryTree", WithOptionalRefParam("-Project", ProjectPath, "-Path", $"'{FilePath}'"));
    }

    private GitlabRepositoryFile? GetFile()
    {
        return Context.GetGitlabObjects(f => new GitlabRepositoryFile(FilesRootPath, f),
                "Get-GitlabRepositoryFile", WithOptionalRefParam("-Project", ProjectPath, "-FilePath", FilePath))
            .FirstOrDefault();
    }

    private bool BranchesPathExists()
    {
        return new BranchPathHandler($"{ProjectPath}/branches/{_branchesPathMatch.Groups["BranchName"].Value}", Context)
            .Exists();
    }

    private string[] WithOptionalRefParam(params string[] args)
    {
        if (_branchesPathMatch.Success)
        {
            return args.Concat(new[] { "-Ref", _branchesPathMatch.Groups["BranchName"].Value }).ToArray();
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