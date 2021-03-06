using Autofac;
using MountAnything;
using MountAnything.Routing;
using MountGitlab.PathHandlers;

namespace MountGitlab;
public class MountGitlabProvider : IMountAnythingProvider
{
    public IEnumerable<DefaultDrive> GetDefaultDrives()
    {
        yield return new DefaultDrive("gitlab")
        {
            Description = "Allows you to navigate gitlab via your GitlabCli configuration"
        };
    }
    
    public Router CreateRouter()
    {
        var router = Router.Create<RootPathHandler>();
        
        router.MapRegex<GroupOrProjectPathHandler>(@"(?<ProjectPath>[a-z0-9-_/\.]+)", project =>
        {
            project.RegisterServices((match, builder) =>
            {
                builder.Register(c => new ProjectPath(new ItemPath(match.Values["ProjectPath"])));
                builder.Register(c => CurrentBranch.Default);
            });
            project.MapLiteral<BranchesPathHandler>("branches", branches =>
            {
                branches.Map<BranchPathHandler,CurrentBranch>(branch =>
                {
                    branch.MapFiles();
                    branch.MapPipelines();
                });
            });
            project.MapLiteral<MergeRequestsPathHandler>("merge-requests", mergeRequests =>
            {
                mergeRequests.Map<MergeRequestPathHandler>();
            });
            project.MapFiles();
            project.MapPipelines();
        });

        return router;
    }
}

internal static class RouteExtensions
{
    public static void MapPipelines(this IRoutable route)
    {
        route.MapLiteral<PipelinesPathHandler>("pipelines", pipelines =>
        {
            pipelines.Map<PipelinePathHandler>(pipeline =>
            {
                pipeline.MapRegex<JobPathHandler>(@"[a-z0-9-_ \.]+");
            });
        });
    }

    public static void MapFiles(this IRoutable route)
    {
        route.MapLiteral<FilesPathHandler>("files", files =>
        {
            files.RegisterServices((match, builder) =>
            {
                builder.Register(c =>
                    match.Values.TryGetValue("FilePath", out var filePath)
                        ? new FilePath(new ItemPath(filePath))
                        : FilePath.Root);
            });
            files.MapRegex<FilesPathHandler>("(?<FilePath>.+)");
        });
    }
}
