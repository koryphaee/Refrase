namespace Refrase.Api.Index;

public record IndexDto(
	int VideoCount,
	int FrameCount,
	int AnalysisQueueLength,
	DateTime LastUpdate);
