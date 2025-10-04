using CliWrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Videos;

public class VideoReEncoder(
	ILogger<VideoReEncoder> logger,
	IDbContextFactory<RefraseContext> contextFactory,
	DataPaths dataPaths,
	IOptionsSnapshot<RefraseOptions> options)
{
	public async Task ReEncode(long videoId, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		Video video = await context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

		if (video.Status != AnalysisStatus.Scheduled)
			return;

		string input = dataPaths.Video(videoId).OriginalVideo;
		string output = dataPaths.Video(videoId).ReEncodedVideo;

		await Cli.Wrap("ffmpeg")
			.WithArguments(a => a
				.Add("-i").Add(input)
				.Add("-sn") // discard subtitles
				.Add("-an") // discard audio
				.AddThreadLimit(options)
				.Add(output))
			.Run(logger, cancellationToken);

		File.Delete(input);

		video.Status = AnalysisStatus.ReEncoded;
		await context.SaveChangesAsync(cancellationToken);
	}
}
