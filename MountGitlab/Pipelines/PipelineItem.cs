using MountAnything;
using MountGitlab.GitlabApi;

namespace MountGitlab.Pipelines;

public class PipelineItem : GitlabItem<Pipeline>
{
    public PipelineItem(ItemPath parentPath, Pipeline pipeline) : base(parentPath, pipeline)
    {
        ItemName = pipeline.PipelineId!;
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}