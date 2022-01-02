using System.Management.Automation;
using MountAnything;

namespace MountGitlab.Models;

public class GitlabRepositoryItem : Item
{
    public static GitlabRepositoryItem FromFile(ItemPath filesRootPath, PSObject fileObject)
    {
        var filePath = fileObject.Property<string>("FilePath")!;

        return new GitlabRepositoryItem(filesRootPath, new ItemPath(filePath), RepositoryItemType.Blob);
    }

    public static GitlabRepositoryItem FromTree(ItemPath filesRootPath, PSObject treeObject)
    {
        var path = treeObject.Property<string>("Path")!;
        var itemType = Enum.Parse<RepositoryItemType>(treeObject.Property<string>("Type")!, ignoreCase: true);

        return new GitlabRepositoryItem(filesRootPath, new ItemPath(path), itemType);
    }
    
    public ItemPath FilesRootPath { get; }
    public RepositoryItemType RepositoryItemType { get; }
    public ItemPath Path { get; }
    
    public GitlabRepositoryItem(ItemPath filesRootPath, ItemPath path, RepositoryItemType itemType) : base(GetParentPath(filesRootPath, path), new PSObject())
    {
        FilesRootPath = filesRootPath;
        Path = path;
        ItemName = path.IsRoot ? filesRootPath.Name : path.Name;
        RepositoryItemType = itemType;
    }

    private static ItemPath GetParentPath(ItemPath rootPath, ItemPath path)
    {
        return rootPath.Combine(path).Parent;
    }
    public override string ItemName { get; }
    public override bool IsContainer => RepositoryItemType == RepositoryItemType.Tree;
    protected override string TypeName => "MountGitlab.GitlabRepositoryItem";
    public override string ItemType => RepositoryItemType switch
    {
        RepositoryItemType.Blob => "File",
        RepositoryItemType.Tree => "Directory",
        _ => throw new ArgumentOutOfRangeException(nameof(RepositoryItemType))
    };

    protected override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(Path), Path.ToString()));
        base.CustomizePSObject(psObject);
    }
}