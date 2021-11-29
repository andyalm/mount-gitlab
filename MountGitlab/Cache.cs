namespace MountGitlab;

public class Cache
{
    private readonly Dictionary<string, GitlabObject> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(GitlabObject item)
    {
        _objects[item.FullPath] = item;
    }

    public IEnumerable<string> AllKeys => _objects.Keys;

    public bool TryGetItem(string path, out GitlabObject cachedItem)
    {
        return _objects.TryGetValue(path, out cachedItem);
    }

    public bool TryGetItem<T>(string path, out T cachedItem) where T : GitlabObject
    {
        if (TryGetItem(path, out var cachedGitObject) && cachedGitObject is T)
        {
            cachedItem = (T)cachedGitObject;
            return true;
        }

        cachedItem = default!;
        return false;
    }
}