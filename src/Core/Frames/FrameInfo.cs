namespace Refrase.Core.Frames;

public record FrameInfo(
	long VideoId,
	int Index,
	TimeSpan Timestamp,
	ulong Hash);
