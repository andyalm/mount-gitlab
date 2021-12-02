using System.Management.Automation;

namespace MountGitlab.Models;

public class GenericGitlabObject : GitlabObject
{
    private readonly GitlabObject _gitlabObject;
    public GenericGitlabObject(GitlabObject gitlabObject) : base(gitlabObject.UnderlyingObject)
    {
        _gitlabObject = gitlabObject;
        _gitlabObject.UnderlyingObject.TypeNames[0] = "MountGitlab.GenericGitlabObject";
    }

    public override string Name => _gitlabObject.Name;
    public override string FullPath => _gitlabObject.FullPath;
    public override bool IsContainer => _gitlabObject.IsContainer;
}