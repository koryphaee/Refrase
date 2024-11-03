using Refrase.Model;

namespace Refrase.Core.Frames;

public record FrameInfo(
	VideoId VideoId,
	int Index,
	TimeSpan Timestamp,
	ulong Hash);
