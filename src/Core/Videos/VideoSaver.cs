using CliWrap;
using Microsoft.Extensions.Logging;
using Refrase.Core.Paths;

namespace Refrase.Core.Videos;

public class VideoSaver(
	ILogger<VideoSaver> logger,
	DataPaths dataPaths)
{
	public async Task<string> Save(Stream stream, CancellationToken cancellationToken)
	{
		Guid guid = Guid.NewGuid();

		string input = dataPaths.Temp.VideoInput(guid);
		await using FileStream target = File.Create(input);
		await stream.CopyToAsync(target, cancellationToken);

		try
		{
			string mp4 = dataPaths.Temp.VideoMp4(guid);
			await Cli
				.Wrap("ffmpeg")
				.WithArguments(a => a
					.Add("-i").Add(input)
					.Add("-sn")
					.Add("-an")
					.Add("-vcodec").Add("copy")
					.Add(mp4))
				.Run(logger, cancellationToken);

			return mp4;
		}
		finally
		{
			File.Delete(input);
		}
	}
}
