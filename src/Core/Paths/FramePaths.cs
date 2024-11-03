namespace Refrase.Core.Paths;

public class FramePaths(string parent) : PathsBase(parent, "frames")
{
	public string Image(Guid guid) => Nested(guid.ToString("N"));
}
