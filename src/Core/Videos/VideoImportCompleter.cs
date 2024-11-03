using Microsoft.EntityFrameworkCore;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Videos;

public class VideoImportCompleter(
	IDbContextFactory<RefraseContext> contextFactory,
	DataPaths dataPaths)
{
	public async Task Complete(VideoId videoId, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		Video video = await context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

		if (video.Status != AnalysisStatus.FramesHashed)
			return;

		string videoDirectory = dataPaths.Video(videoId).Directory;
		Directory.Delete(videoDirectory, true);

		video.Status = AnalysisStatus.Completed;
		await context.SaveChangesAsync(cancellationToken);
	}
}
