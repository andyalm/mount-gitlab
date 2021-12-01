using System.Management.Automation.Provider;

namespace MountGitlab;

public interface ISupportContentReader
{
    IContentReader GetContentReader();
}