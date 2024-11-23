namespace Refrase.Core.Videos;

public record CreateVideoRequest(
	string Name,
	string Category,
	string? Url,
	string Path);
