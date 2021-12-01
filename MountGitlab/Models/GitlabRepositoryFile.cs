using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabRepositoryFile : GitlabObject
{
    public string FilesPath { get; set; }
    
    public GitlabRepositoryFile(string filesPath, PSObject underlyingObject) : base(underlyingObject)
    {
        FilesPath = filesPath;
    }

    public override string Name => Property<string>("FileName");
    public override string FullPath => GitlabPath.Combine(FilesPath, FilePath);
    public override bool IsContainer => false;
    public string FilePath => Property<string>("FilePath");

    public string Encoding => Property<string>("Encoding");

    public string Base64Content => Property<string>("Content");
}