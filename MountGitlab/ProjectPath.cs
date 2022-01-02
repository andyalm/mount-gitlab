using MountAnything;

namespace MountGitlab;

public record ProjectPath(ItemPath ItemPath) : TypedItemPath(ItemPath)
{
    public override string ToString() => ItemPath.ToString();
}