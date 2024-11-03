using Refrase.Core.Metadata.Frames;

namespace Refrase.Core.Frames;

public class ExtraFrameCreator
{
	public FrameMetadata[] CreateExtraFrames(FrameMetadata[] frames)
	{
		return frames.First().Timestamp.TotalMilliseconds < 0.1
			? frames
			: FillFrames(frames).ToArray();
	}

	private static IEnumerable<FrameMetadata> FillFrames(FrameMetadata[] frames)
	{
		double averageFrameTime = frames
			.Zip(frames.Skip(1), (a, b) => b.Timestamp.TotalMilliseconds - a.Timestamp.TotalMilliseconds)
			.Average();

		int extraFrames = (int) Math.Round(frames.First().Timestamp.TotalMilliseconds / averageFrameTime);

		for (int i = 0; i < extraFrames; i++)
		{
			yield return new FrameMetadata(i, TimeSpan.FromMilliseconds(averageFrameTime * i));
		}

		foreach (FrameMetadata frame in frames)
		{
			yield return new FrameMetadata(frame.Index + extraFrames, frame.Timestamp);
		}
	}
}
