namespace Refrase.Api.Frames.SearchFrame;

public record SearchFrameResponse(
	MatchDto? Match,
	ulong InputHash);
