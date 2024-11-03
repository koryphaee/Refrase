using System.Text.Json.Serialization;

namespace Refrase.Core.Metadata.Frames;

internal record FfprobeFramesMetadata(
	[property: JsonPropertyName("frames")] FfprobeFrameMetadata[] Frames);
