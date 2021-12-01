using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabJob : GitlabObject
{
    public string ContainerPath { get; }
    private string NameProperty { get; }
    
    public GitlabJob(string containerPath, PSObject underlyingObject, string nameProperty = "Name") : base(underlyingObject)
    {
        ContainerPath = containerPath;
        NameProperty = nameProperty;
    }

    public long Id => Property<long>("Id");
    
    public string JobName => Property<string>("Name");
    public string ProjectId => Property<object>("ProjectId").ToString()!;
    public override string Name => Property<object>(NameProperty).ToString()!;
    public override string FullPath => $"{ContainerPath}/{Name}";
    public override bool IsContainer => false;
}