namespace Refrase.Core.Paths;

public class TempPaths(string parent) : PathsBase(parent, "temp")
{
	public string Video(Guid guid) => Nested(guid.ToString("N"));
}
