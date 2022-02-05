using System.Text.Json.Serialization;

namespace MountGitlab.GitlabApi;

public record GraphQLCollection<T>
{
    public T[]? Nodes { get; init; }
}