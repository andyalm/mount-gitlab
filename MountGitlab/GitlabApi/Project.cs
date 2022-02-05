using System.Text.Json.Serialization;

namespace MountGitlab.GitlabApi;

public record Project
{
    public GraphQLCollection<Pipeline>? Pipelines { get; init; }
}