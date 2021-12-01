namespace MountGitlab;

public interface IPathHandler
{
    string Path { get; }
    bool Exists();
    GitlabObject? GetItem();

    IEnumerable<GitlabObject> GetChildItems(bool recurse);
}