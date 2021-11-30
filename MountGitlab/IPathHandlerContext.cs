using System.Collections.ObjectModel;
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
        return context.GetGitlabObjects(g => new GitlabGroup(g), "Get-GitlabGroup", args);
    }

    public static IEnumerable<GitlabProject> GetProjects(this IPathHandlerContext context, params string[] args)
    {
        return context.GetGitlabObjects(p => new GitlabProject(p), "Get-GitlabProject", args);
    }

    public static IEnumerable<T> GetGitlabObjects<T>(this IPathHandlerContext context,
        Func<PSObject, T> create,
        string commandName,
        params string[] args) where T : GitlabObject
    {
        return context.GetGitlabObjects(create, _ => true, commandName, args);
    }

    public static IEnumerable<T> GetGitlabObjects<T>(this IPathHandlerContext context,
        Func<PSObject, T> create,
        Func<T, bool> validate,
        string commandName,
        params string[] args) where T : GitlabObject
    {
        Collection<PSObject>? response = default;
        try
        {
            var fullCommand = $"{commandName} {string.Join(" ", args)}";
            context.WriteDebug(fullCommand);
            response = context.InvokeCommand.InvokeScript(fullCommand);
        }
        catch (CmdletInvocationException ex) when (ex.Message.Contains("404"))
        {
            yield break;
        }
        catch (Exception ex)
        {
            context.WriteDebug(ex.ToString());
        }

        if (response != null)
        {
            foreach (var item in response)
            {
                var gitlabObject = create(item);
                if (validate(gitlabObject))
                {
                    context.Cache.SetItem(gitlabObject);

                    yield return gitlabObject;
                }
            }
        }
    }
}