using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabMergeRequest : GitlabObject
{
    public GitlabMergeRequest(PSObject underlyingObject) : base(underlyingObject)
    {
    }

    public override string Name => Iid.ToString();
    public override string FullPath => $"{ProjectPath}/merge-requests/{Name}";
    public override bool IsContainer => false;
    public long Iid => Property<long>("Iid");
    public string ProjectPath => Property<object>("ProjectName").ToString()!;
}