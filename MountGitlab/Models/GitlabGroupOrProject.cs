using System.Management.Automation;

namespace MountGitlab.Models;

public class GitlabGroupOrProject : GitlabObject
{
    private readonly GitlabObject _gitlabObject;

    public GitlabGroupOrProject(GitlabProject project) : this((GitlabObject)project) {}
    public GitlabGroupOrProject(GitlabGroup group) : this((GitlabObject)group) {}
    protected GitlabGroupOrProject(GitlabObject gitlabObject) : base(gitlabObject.UnderlyingObject)
    {
        _gitlabObject = gitlabObject;
        _gitlabObject.UnderlyingObject.TypeNames[0] = "MountGitlab.GitlabGroupOrProject";
    }

    public override string Name => _gitlabObject.Name;
    public override string FullPath => _gitlabObject.FullPath;
    public override bool IsContainer => _gitlabObject.IsContainer;
}

public static class GitlabGroupOrProjectExtensions
{
    public static IEnumerable<GitlabGroupOrProject> ToGitlabGroupOrProjects(this IEnumerable<GitlabProject> projects)
    {
        return projects.Select(p => new GitlabGroupOrProject(p));
    }

    public static IEnumerable<GitlabGroupOrProject> ToGitlabGroupOrProjects(this IEnumerable<GitlabGroup> groups)
    {
        return groups.Select(g => new GitlabGroupOrProject(g));
    }

}