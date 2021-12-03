using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabRepositoryTree : GitlabObject
{
    public static GitlabRepositoryTree FromGitlabFile(string filesPath, PSObject fileObject)
    {
        var tree = new
        {
            Name = (string)fileObject.Properties["FileName"].Value,
            Type = "blob",
            Path = (string)fileObject.Properties["FilePath"].Value
        };
        return new GitlabRepositoryTree(filesPath, new PSObject(tree));
    }
    
    public string FilesPath { get; }
    
    public GitlabRepositoryTree(string filesPath, PSObject underlyingObject) : base(underlyingObject)
    {
        if (!underlyingObject.TypeNames.Any())
        {
            underlyingObject.TypeNames.Add("Gitlab.RepositoryTree");
        }
        FilesPath = filesPath;
    }

    public override string Name => Property<string>("Name");
    public override string FullPath => GitlabPath.Combine(FilesPath, Path);
    public override bool IsContainer => Type == "tree";
    public string Type => Property<string>("Type");

    public string Path => Property<string>("Path");
}