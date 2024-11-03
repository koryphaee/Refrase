using System.Text.Json.Serialization;

namespace Refrase.Core.Metadata.Videos;

internal record FfprobeStreamMetadata(
	[property: JsonPropertyName("width")] int Width,
	[property: JsonPropertyName("height")] int Height,
	[property: JsonPropertyName("r_frame_rate")] string FrameRate,
	[property: JsonPropertyName("nb_frames")] string FrameCount,
	[property: JsonPropertyName("duration")] string Duration);
