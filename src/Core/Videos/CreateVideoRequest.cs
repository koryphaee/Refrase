namespace Refrase.Core.Videos;

public record CreateVideoRequest(
	string Name,
	string Category,
	string? Url,
	Stream Content);
