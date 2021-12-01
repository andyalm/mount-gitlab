using System.Text;
using MountGitlab.Models;

namespace MountGitlab;

public class JobContentReader : MemoryStreamContentReader
{
    private readonly GitlabJob _job;
    private readonly IPathHandlerContext _context;

    public JobContentReader(GitlabJob job, IPathHandlerContext context)
    {
        _job = job;
        _context = context;
    }

    // It would be better to just retrieve the trace directly, but this approach leaves the get-content
    // method hanging for some reason
    //
    // public void Dispose() { }
    //
    // public void Close() { }
    //
    // public IList Read(long readCount)
    // {
    //     return _context.InvokeCommand.InvokeScript($"Get-GitlabJobTrace -ProjectId {_job.ProjectId} -JobId {_job.Id}")
    //         .Concat(new []{new PSObject("")}).ToList();
    // }
    //
    // public void Seek(long offset, SeekOrigin origin) {}
    protected override MemoryStream CreateContentStream()
    {
        var traceAsString = _context.InvokeCommand
            .InvokeScript($"Get-GitlabJob -ProjectId {_job.ProjectId} -JobId {_job.Id} -IncludeTrace")
            .First()
            .Properties["Trace"].ToString()!;

        return new MemoryStream(Encoding.UTF8.GetBytes(traceAsString));
    }
}