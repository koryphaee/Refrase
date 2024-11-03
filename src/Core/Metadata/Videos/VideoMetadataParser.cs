namespace Refrase.Core.Metadata.Videos;

internal class VideoMetadataParser
{
	public VideoMetadata Parse(FfprobeVideoMetadata video)
	{
		if (video.Streams.Length != 1)
			throw new RefraseException($"Invalid amount of streams {video.Streams.Length}");

		FfprobeStreamMetadata stream = video.Streams.Single();

		int frameRate = GetFrameRate(stream.FrameRate);
		int frameCount = stream.FrameCount.Parse<int>("frame count");
		TimeSpan duration = GetDuration(stream.Duration);
		return new VideoMetadata(stream.Width, stream.Height, frameRate, frameCount, duration);
	}

	private static int GetFrameRate(string frameRate)
	{
		string[] parts = frameRate.Split('/');

		if (parts.Length != 2)
			throw new RefraseException($"Unknown frame rate format '{frameRate}'");

		int dividend = parts[0].Parse<int>("frame rate dividend");
		int divisor = parts[1].Parse<int>("frame rate divisor");
		return (int) Math.Round((float) dividend / divisor);
	}

	private static TimeSpan GetDuration(string duration)
	{
		float seconds = duration.Parse<float>("duration");
		return TimeSpan.FromSeconds(seconds);
	}
}
