using Refrase.Core.Paths;

namespace Refrase.Core.Videos;

public class VideoSaver(
	DataPaths dataPaths)
{
	public async Task<string> Save(Stream stream, CancellationToken cancellationToken)
	{
		Guid guid = Guid.NewGuid();
		string path = dataPaths.Temp.Video(guid);
		await using FileStream target = File.Create(path);
		await stream.CopyToAsync(target, cancellationToken);
		return path;
	}
}
