using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Refrase.Api.Videos.IngestVideo;

public record IngestVideoRequest(
	[FromForm] string Name,
	[FromForm] string Category,
	[FromForm] string? Url,
	IFormFile Video);
