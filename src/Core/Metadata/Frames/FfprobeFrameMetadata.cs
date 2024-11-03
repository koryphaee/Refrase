using System.Text.Json.Serialization;

namespace Refrase.Core.Metadata.Frames;

internal record FfprobeFrameMetadata(
	[property: JsonPropertyName("best_effort_timestamp_time")] string Timestamp);
