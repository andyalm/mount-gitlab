using System.Collections;
using System.Management.Automation.Provider;
using MountGitlab.Models;

namespace MountGitlab;

public class RepositoryFileReader : IContentReader
{
    private Lazy<MemoryStream> _contentStream;
    private readonly GitlabRepositoryFile _file;

    public RepositoryFileReader(GitlabRepositoryFile file)
    {
        _file = file;
        _contentStream = new Lazy<MemoryStream>(CreateContentStream);
    }

    private MemoryStream CreateContentStream()
    {
        if (_file.Encoding == "base64")
        {
            return new MemoryStream(Convert.FromBase64String(_file.Base64Content));
        }

        throw new NotSupportedException($"File encoding '{_file.Encoding}' is not currently supported");
    }

    public void Dispose()
    {
        Close();
    }

    public void Close()
    {
        if (_contentStream.IsValueCreated)
        {
            _contentStream.Value.Close();
            _contentStream = new Lazy<MemoryStream>(CreateContentStream);
        }
    }

    public IList Read(long readCount)
    {
        var blocks = new List<string>();

        var reader = new StreamReader(_contentStream.Value);
        // It is observed that displaying content can be slow especially on xterm cases.
        // Thus by default, we read them all and return all once. This means the user cannot use
        // Get-Content -TotalCount feature, which is fine comparing the speed of displaying content output.
        var content = reader.ReadToEnd();
        if (content.Length > 0)
        {
            // For some reason ReadToEnd() or Readline() inserts LF at the end. So trim them off here.
            blocks.Add(content.TrimEnd(Environment.NewLine.ToCharArray()));
        }

        return blocks;
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        _contentStream.Value.Seek(offset, origin);
    }
}