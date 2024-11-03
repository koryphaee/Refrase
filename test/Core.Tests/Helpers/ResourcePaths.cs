using Refrase.Core.Paths;

namespace Refrase.Core.Tests.Helpers;

public class ResourcePaths() : PathsBase("../../../../../res", "")
{
	public string Frame(int index) => Nested($"frame{index}.jpg");

	public string Video => Nested("video.mp4");
}

