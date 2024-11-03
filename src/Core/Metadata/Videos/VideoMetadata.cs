namespace Refrase.Core.Metadata.Videos;

public record VideoMetadata(
	int Width,
	int Height,
	int FrameRate,
	int FrameCount,
	TimeSpan Duration);
