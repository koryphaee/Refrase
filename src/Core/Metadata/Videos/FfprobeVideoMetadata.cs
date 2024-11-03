using System.Text.Json.Serialization;

namespace Refrase.Core.Metadata.Videos;

internal record FfprobeVideoMetadata(
	[property: JsonPropertyName("streams")] FfprobeStreamMetadata[] Streams);
