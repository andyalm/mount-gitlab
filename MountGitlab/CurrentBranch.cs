using MountAnything;

namespace MountGitlab;

public record CurrentBranch(string Value) : TypedString(Value)
{
    public static CurrentBranch Default { get; } = new(string.Empty);
    
    public bool IsDefault = string.IsNullOrEmpty(Value);
}