using System.Management.Automation;
using MountGitlab.Models;

namespace MountGitlab;

public interface IPathHandlerContext
{
    CommandInvocationIntrinsics InvokeCommand { get; }
    
    Cache Cache { get; }

    void WriteDebug(string message);
}

public static class PathHandlerContextExtensions
{
    public static IEnumerable<GitlabGroup> GetGroups(this IPathHandlerContext context, params string[] args)
    {
        context.WriteDebug($"Get-GitlabGroup {string.Join(" ", args)}");
        var response = context.InvokeCommand.InvokeScript($"Get-GitlabGroup {string.Join(" ", args)}");
        if (response == null)
        {
            throw new InvalidOperationException("Unexpected response from Get-GitlabGroup");
        }

        foreach (var rawGroup in response)
        {
            var group = new GitlabGroup(rawGroup);
            context.Cache.SetItem(group);

            yield return group;
        }
    }

    public static IEnumerable<GitlabProject> GetProjects(this IPathHandlerContext context, params string[] args)
    {
        context.WriteDebug($"Get-GitlabProject {string.Join(" ", args)}");
        var response = context.InvokeCommand.InvokeScript($"Get-GitlabProject {string.Join(" ", args)}");
        if (response == null)
        {
            throw new InvalidOperationException("Unexpected response from Get-GitlabProject");
        }
        foreach (var rawProject in response)
        {
            var project = new GitlabProject(rawProject);
            context.Cache.SetItem(project);

            yield return project;
        }
    }
}