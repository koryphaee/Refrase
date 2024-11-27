using Microsoft.EntityFrameworkCore;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Metadata.Videos;

internal class VideoMetadataImporter(
	VideoMetadataReader videoMetadataReader,
	IDbContextFactory<RefraseContext> contextFactory,
	DataPaths dataPaths)
{
	public async Task Import(long videoId, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		Video video = await context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

		if (video.Status != AnalysisStatus.Scheduled)
			return;

		string path = dataPaths.Video(videoId).Video;
		VideoMetadata metadata = await videoMetadataReader.ExtractMetadata(path, cancellationToken);
		video.Width = metadata.Width;
		video.Height = metadata.Height;
		video.Duration = metadata.Duration;
		video.FrameCount = metadata.FrameCount;
		video.FrameRate = metadata.FrameRate;
		video.Status = AnalysisStatus.MetadataImported;
		await context.SaveChangesAsync(cancellationToken);
	}
}
