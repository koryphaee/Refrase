using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using Refrase.Core.Frames;
using System.Text.Json;

namespace Refrase.Core.Metadata.Frames;

internal class FrameMetadataReader(
	ILogger<FrameMetadataReader> logger,
	FrameMetadataParser frameMetadataParser)
{
	public async Task<FrameMetadata[]> ExtractMetadata(string path, CancellationToken cancellationToken)
	{
		string json = await RunCommand(path, cancellationToken);
		FfprobeFramesMetadata metadata = Deserialize(json);
		return frameMetadataParser.Parse(metadata);
	}

	private async Task<string> RunCommand(string path, CancellationToken cancellationToken)
	{
		BufferedCommandResult result = await Cli
			.Wrap("ffprobe")
			.WithArguments(a => a
				.Add("-select_streams").Add("v:0")
				.Add("-show_frames")
				.Add("-show_entries").Add("frame=best_effort_timestamp_time")
				.Add("-output_format").Add("json")
				.Add("-i").Add(path))
			.RunBuffered(logger, cancellationToken);
		return result.StandardOutput;
	}

	private FfprobeFramesMetadata Deserialize(string json)
	{
		FfprobeFramesMetadata? metadata;

		try
		{
			metadata = JsonSerializer.Deserialize<FfprobeFramesMetadata>(json);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error parsing output {json}", json);
			throw new RefraseException("Error deserializing metadata", e);
		}

		if (metadata == null)
			throw new RefraseException("Deserialized metadata is null");

		return metadata;
	}
}
