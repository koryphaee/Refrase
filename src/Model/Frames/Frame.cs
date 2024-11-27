using Refrase.Model.Videos;

namespace Refrase.Model.Frames;

public class Frame
{
	public long Id { get; set; }

	public long VideoId { get; set; }

	public Video? Video { get; set; }

	public required int Index { get; set; }

	public required TimeSpan Timestamp { get; set; }

	public ulong? Hash { get; set; }
}
