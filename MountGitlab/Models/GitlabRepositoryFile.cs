using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabRepositoryFile
{
    private PSObject UnderlyingObject { get; }
    
    public GitlabRepositoryFile(PSObject underlyingObject)
    {
        UnderlyingObject = underlyingObject;
    }
    
    public string Encoding => UnderlyingObject.Property<string>("Encoding")!;
    public string Base64Content => UnderlyingObject.Property<string>("Content")!;
}