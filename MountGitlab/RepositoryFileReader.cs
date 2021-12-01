using MountGitlab.Models;

namespace MountGitlab;

public class RepositoryFileReader : MemoryStreamContentReader
{
    private readonly GitlabRepositoryFile _file;

    public RepositoryFileReader(GitlabRepositoryFile file)
    {
        _file = file;
    }

    protected override MemoryStream CreateContentStream()
    {
        if (_file.Encoding == "base64")
        {
            return new MemoryStream(Convert.FromBase64String(_file.Base64Content));
        }

        throw new NotSupportedException($"File encoding '{_file.Encoding}' is not currently supported");
    }
}