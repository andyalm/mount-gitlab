using MountAnything;

namespace MountGitlab;

public record FilePath(ItemPath ItemPath) : TypedItemPath(ItemPath)
{
    public static FilePath Root { get; } = new(ItemPath.Root);

    public override string ToString() => ItemPath.ToString();
}