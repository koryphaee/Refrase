namespace Refrase.Core.Paths;

public class TempPaths(string parent) : PathsBase(parent, "temp")
{
	public string VideoInput(Guid guid) => Nested(guid.ToString("N"));
	
	public string VideoMp4(Guid guid) => Nested(guid.ToString("N") + ".mp4");
}
