using System.Text.Json.Serialization;

namespace MountGitlab.GitlabApi;

public record Response
{
    public Project? Project { get; init; }
}