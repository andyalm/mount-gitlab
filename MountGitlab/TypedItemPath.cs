using MountAnything;

namespace MountGitlab;

public abstract record TypedItemPath(ItemPath ItemPath)
{
    public bool IsRoot => ItemPath.IsRoot;

    public override string ToString() => ItemPath.ToString();
}