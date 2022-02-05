using System.Text.Json.Serialization;

namespace MountGitlab.GitlabApi;

public record Pipeline
{
    public string? Id { get; init; }

    public string? PipelineId => Id?.Split("/")[^1];
    
    public string? Ref { get; init; }
    public string? Sha { get; init; }

    public string? Status { get; init; }
    public PipelineUser? User { get; init; }
    public bool? Active { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }
    public string? Path { get; init; }
    public bool? UsesNeeds { get; init; }
    public bool? Warnings { get; init; }

    public class PipelineUser
    {
        public string? Username { get; init; }
        public string? Name { get; init; }
    }
}