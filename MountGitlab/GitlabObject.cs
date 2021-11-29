using System.Management.Automation;

namespace MountGitlab;

public abstract class GitlabObject
{
    protected GitlabObject(PSObject underlyingObject)
    {
        UnderlyingObject = underlyingObject;
    }
    
    public PSObject UnderlyingObject { get; }
    
    public abstract string Name { get; }
    
    public abstract string FullPath { get; }
    
    public abstract bool IsContainer { get; }

    public T Property<T>(string propertyName)
    {
        return (T)UnderlyingObject.Properties[propertyName].Value;
    }
}