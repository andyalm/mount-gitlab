namespace MountGitlab;

public class Cache
{
    private readonly Dictionary<string, CachedItem> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(GitlabObject item)
    {
        if(_objects.TryGetValue(item.FullPath, out var cachedItem))
        {
            cachedItem.Item = item;
        }
        else
        {
            _objects[item.FullPath] = new CachedItem(item);
        }
    }

    public bool TryGetItem(string path, out GitlabObject cachedObject)
    {
        if (_objects.TryGetValue(path, out var cachedItem))
        {
            cachedObject = cachedItem.Item;
            return true;
        }

        cachedObject = default!;
        return false;
    }

    public bool TryGetItem<T>(string path, out T cachedItem) where T : GitlabObject
    {
        if (TryGetItem(path, out var cachedGitObject) && cachedGitObject is T cachedTypedObject)
        {
            cachedItem = cachedTypedObject;
            return true;
        }

        cachedItem = default!;
        return false;
    }
    
    public void SetChildItems(GitlabObject item, IEnumerable<GitlabObject> childItems)
    {
        SetItem(item);
        foreach (var childItem in childItems)
        {
            SetItem(childItem);
        }
        var cachedItem = _objects[item.FullPath];
        cachedItem.ChildPaths = childItems.Select(i => i.FullPath).ToList();
    }

    public bool TryGetChildItems(string path, out IEnumerable<GitlabObject> childItems)
    {
        if (_objects.TryGetValue(path, out var cachedItem) && cachedItem.ChildPaths != null)
        {
            childItems = cachedItem.ChildPaths.Select(childPath => _objects[childPath].Item).ToArray();
            return true;
        }

        childItems = default!;
        return false;
    }
    
    private class CachedItem
    {
        public CachedItem(GitlabObject item)
        {
            Item = item;
            ChildPaths = null;
        }
        
        public GitlabObject Item { get; set; }
        public List<string>? ChildPaths { get; set; }
    }
}