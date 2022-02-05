using System.Text.Json;
using GraphQL;
using MountAnything;
using MountGitlab.GitlabApi;
using MountGitlab.Models;
using MountGitlab.Pipelines;

namespace MountGitlab.PathHandlers;

public class PipelinesPathHandler : PathHandler
{
    public ProjectPath ProjectPath { get; }
    public CurrentBranch CurrentBranch { get; }
    
    public PipelinesPathHandler(ItemPath path, IPathHandlerContext context, ProjectPath projectPath, CurrentBranch currentBranch) : base(path, context)
    {
        ProjectPath = projectPath;
        CurrentBranch = currentBranch;
    }

    protected override bool ExistsImpl()
    {
        return CurrentBranch.IsDefault
            ? new GroupOrProjectPathHandler(ProjectPath.ItemPath, Context).GetProject() != null
            : new BranchPathHandler(ParentPath, Context).Exists();
    }

    protected override IItem GetItemImpl()
    {
        return CurrentBranch.IsDefault ?
            new ProjectSection(ProjectPath.ItemPath, "pipelines") :
            new RefSection(ProjectPath.ItemPath, CurrentBranch.Value, "pipelines");
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var request = new GraphQLRequest
        {
            Query =
                @"query($projectPath: ID!, $commitRef: String) {
                    project(fullPath: $projectPath) {
                        pipelines(ref: $commitRef) {
                            nodes {
                                id
                                ref
                                status
                                user {
                                  username
                                  name
                                }
                                active
                                startedAt
                                createdAt
                                finishedAt
                                path
                                sha
                                usesNeeds
                                warnings
                            }
                        }
                    }
                }",
            Variables = new
            {
                projectPath = ProjectPath.ToString(),
                commitRef = CurrentBranch.IsDefault ? null : CurrentBranch.Value
            }
        };
        var response = Context.GraphQLQuery<Response>(request);

        return response.Project!.Pipelines!.Nodes!.Select(n => new PipelineItem(Path, n));
    }
}