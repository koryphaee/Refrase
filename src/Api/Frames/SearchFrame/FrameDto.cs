namespace Refrase.Api.Frames.SearchFrame;

public record FrameDto(
	int Index,
	TimeSpan Time,
	ulong Hash,
	double Similarity);
