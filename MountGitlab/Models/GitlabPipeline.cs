using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabPipeline : GitlabObject
{
    public string ContainerPath { get; }
    
    public GitlabPipeline(string containerPath, PSObject underlyingObject) : base(underlyingObject)
    {
        ContainerPath = containerPath;
    }

    public override string Name => Property<long>("Id").ToString();
    public override string FullPath => $"{ContainerPath}/{Name}";
    public override bool IsContainer => false;

    public string Ref => Property<string>("Ref");
}