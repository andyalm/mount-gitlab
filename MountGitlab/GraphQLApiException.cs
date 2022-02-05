using System.Text.Json;
using GraphQL;

namespace MountGitlab;

public class GraphQLApiException : Exception
{
    public GraphQLError[] Errors { get; }
    
    public GraphQLApiException(GraphQLError[] errors)
    {
        Errors = errors;
        Message =
            $"The following errors occurred with he graphql request: {JsonSerializer.Serialize(errors, new JsonSerializerOptions { WriteIndented = true })}";
    }

    public override string Message { get; }
}