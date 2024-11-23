using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using Refrase.Core.Metadata.Frames;
using System.Text.Json;

namespace Refrase.Core.Videos;

public class VideoValidator(
	ILogger<VideoValidator> logger)
{
	public async Task<bool> Validate(string path, CancellationToken cancellationToken)
	{
		try
		{
			BufferedCommandResult result = await Cli
				.Wrap("ffprobe")
				.WithArguments(a => a
					.Add("-select_streams").Add("v:0")
					.Add("-show_frames")
					.Add("-show_entries").Add("frame=best_effort_timestamp_time")
					.Add("-read_intervals").Add("%+#1") // only the first frame
					.Add("-output_format").Add("json")
					.Add("-i").Add(path))
				.Run(logger, cancellationToken);
			FfprobeFramesMetadata metadata = JsonSerializer.Deserialize<FfprobeFramesMetadata>(result.StandardOutput)!;
			FfprobeFrameMetadata firstFrame = metadata.Frames.Single();
			float timestamp = firstFrame.Timestamp.Parse<float>("first frame timestamp");

			if (timestamp != 0)
				logger.LogWarning("Rejecting video because the first frame timestamp is {timestamp}", timestamp);

			return timestamp == 0;
		}
		catch (Exception e)
		{
			logger.LogWarning(e, "Error validating video file");
			return false;
		}
	}
}
