namespace Refrase.Core.Metadata.Frames;

internal class FrameMetadataParser
{
	public FrameMetadata[] Parse(FfprobeFramesMetadata frames)
	{
		if (!frames.Frames.Any())
			throw new RefraseException($"No frames available");

		return frames.Frames
			.Select(GetDuration)
			.ToArray();
	}

	private static FrameMetadata GetDuration(FfprobeFrameMetadata frame, int index)
	{
		float seconds = frame.Timestamp.Parse<float>("timestamp");
		var timestamp = TimeSpan.FromSeconds(seconds);
		return new FrameMetadata(index, timestamp);
	}
}
