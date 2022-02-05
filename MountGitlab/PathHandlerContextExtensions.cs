using System.Management.Automation;
using System.Net.Http.Headers;
using System.Text.Json;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
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

    public static T GraphQLQuery<T>(this IPathHandlerContext context, GraphQLRequest request)
    {
        using var client = new GraphQLHttpClient(options => ConfigureGraphQLClient(options, context), new SystemTextJsonSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        var apiToken = Environment.GetEnvironmentVariable("GITLAB_ACCESS_TOKEN");
        if (apiToken != null)
        {
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        }
        var response = client.SendQueryAsync<T>(request).GetAwaiter().GetResult();
        context.WriteDebug(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        if (response.Errors?.Any() == true)
        {
            throw new GraphQLApiException(response.Errors);
        }

        return response.Data;
    }

    private static void ConfigureGraphQLClient(GraphQLHttpClientOptions options, IPathHandlerContext context)
    {
        var endpoint = Environment.GetEnvironmentVariable("GITLAB_URL") ?? "https://gitlab.com";
        if (!endpoint.EndsWith("/"))
        {
            endpoint += "/";
        }
        endpoint += "api/graphql";
        options.EndPoint = new Uri(endpoint);
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