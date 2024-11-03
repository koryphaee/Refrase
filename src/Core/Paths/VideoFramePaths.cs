namespace Refrase.Core.Paths;

public class VideoFramePaths(string parent) : PathsBase(parent, "frames")
{
	public string Pattern => Nested("%010d.jpg");

	public string Frame(int index) => Nested($"{index:D10}.jpg");
}
