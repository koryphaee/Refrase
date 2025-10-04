using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Refrase.Core.Metadata.Videos;

internal class VideoMetadataReader(
	ILogger<VideoMetadataReader> logger,
	VideoMetadataParser videoMetadataParser)
{
	public async Task<VideoMetadata> ExtractMetadata(string path, CancellationToken cancellationToken)
	{
		string json = await RunCommand(path, cancellationToken);
		FfprobeVideoMetadata metadata = Deserialize(json);
		return videoMetadataParser.Parse(metadata);
	}

	private async Task<string> RunCommand(string path, CancellationToken cancellationToken)
	{
		BufferedCommandResult result = await Cli
			.Wrap("ffprobe")
			.WithArguments(a => a
				.Add("-i").Add(path)
				.Add("-select_streams").Add("v:0")
				.Add("-show_streams")
				// can be used to filter down the output fields, but it saves no time
				//.Add("-show_entries stream=width,height,r_frame_rate,nb_frames,duration")
				.Add("-loglevel").Add("quiet")
				.Add("-output_format").Add("json"))
			.RunBuffered(logger, cancellationToken);
		return result.StandardOutput;
	}

	private FfprobeVideoMetadata Deserialize(string json)
	{
		FfprobeVideoMetadata? metadata;

		try
		{
			metadata = JsonSerializer.Deserialize<FfprobeVideoMetadata>(json);
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
