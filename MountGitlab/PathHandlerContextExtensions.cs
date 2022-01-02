using System.Management.Automation;
using MountAnything;
using MountGitlab.Models;

namespace MountGitlab;

public static class PathHandlerContextExtensions
{
    public static IEnumerable<GroupOrProjectItem> GetGroups(this IPathHandlerContext context, params string[] args)
    {
        return context.GetItems(GroupOrProjectItem.ForGroup, "Get-GitlabGroup", args);
    }

    public static IEnumerable<GroupOrProjectItem> GetProjects(this IPathHandlerContext context, params string[] args)
    {
        return context.GetItems(GroupOrProjectItem.ForProject, "Get-GitlabProject", args);
    }

    public static IEnumerable<T> GetItems<T>(this IPathHandlerContext context,
        Func<PSObject, T> create,
        string commandName,
        params string[] args) where T : IItem
    {
        return context.GetItems(create, _ => true, commandName, args);
    }

    public static IEnumerable<T> GetItems<T>(this IPathHandlerContext context,
        Func<PSObject, T> create,
        Func<T, bool> validate,
        string commandName,
        params string[] args) where T : IItem
    {
        var psObjects = context.GetPSObjects(commandName, args);
        foreach (var item in psObjects)
        {
            var gitlabObject = create(item);
            if (validate(gitlabObject))
            {
                //context.Cache.SetItem(gitlabObject);

                yield return gitlabObject;
            }
        }
    }
    
    public static IEnumerable<PSObject> GetPSObjects(this IPathHandlerContext context, string commandName, params string[] args)
    {
        try
        {
            var fullCommand = $"{commandName} {string.Join(" ", args)}";
            context.WriteDebug(fullCommand);
            return context.InvokeCommand.InvokeScript(fullCommand);
        }
        catch (CmdletInvocationException ex) when (ex.Message.Contains("404"))
        {
            return Enumerable.Empty<PSObject>();
        }
        catch (Exception ex)
        {
            context.WriteDebug(ex.ToString());
            return Enumerable.Empty<PSObject>();
        }
    }
}