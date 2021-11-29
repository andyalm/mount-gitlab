namespace MountGitlab;

public interface IPathHandler
{
    bool Exists();
    
    GitlabObject GetItem();

    IEnumerable<GitlabObject> GetChildItems(bool recurse);
}