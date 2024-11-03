using Refrase.Model;

namespace Refrase.Core.Paths;

public class DataPaths(MappedPaths mappedPaths) : PathsBase(mappedPaths.Data, "")
{
	public VideoPaths Video(VideoId id) => new(Directory, id);

	public FramePaths Frame => new(Directory);

	public string Database => Nested("database.sqlite");
}
