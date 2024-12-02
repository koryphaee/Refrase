namespace Refrase.Core.Paths;

public class VideoPaths(string parent, long id) : PathsBase(parent, id.ToString())
{
	public string OriginalVideo => Nested("video.original");

	public string ReEncodedVideo => Nested("video.mp4");

	public string FrameList => Nested("frame_list.json");

	public VideoFramePaths Frames => new(Directory);
}
