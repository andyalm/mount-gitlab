using MountAnything;

namespace MountGitlab;

public abstract class GitlabItem<T> : Item<T> where T : class
{
    protected GitlabItem(ItemPath parentPath, T underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    protected override string TypeName => GetType().FullName!;
}