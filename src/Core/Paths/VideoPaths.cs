using Refrase.Model;

namespace Refrase.Core.Paths;

public class VideoPaths(string parent, long id) : PathsBase(parent, id.ToString())
{
	public string Video => Nested("video");

	public string FrameList => Nested("frame_list.json");

	public VideoFramePaths Frames => new(Directory);
}
