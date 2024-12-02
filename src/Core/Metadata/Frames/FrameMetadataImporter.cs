using Microsoft.EntityFrameworkCore;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Metadata.Frames;

internal class FrameMetadataImporter(
	FrameMetadataReader frameMetadataReader,
	IDbContextFactory<RefraseContext> contextFactory,
	DataPaths dataPaths)
{
	public async Task Import(long videoId, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		Video video = await context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

		if (video.Status != AnalysisStatus.MetadataImported)
			return;

		string path = dataPaths.Video(videoId).ReEncodedVideo;
		FrameMetadata[] frames = await frameMetadataReader.ExtractMetadata(path, cancellationToken);

		foreach (FrameMetadata metadata in frames)
		{
			var frame = new Frame
			{
				Video = video,
				Index = metadata.Index,
				Timestamp = metadata.Timestamp
			};
			context.Add(frame);
		}

		video.Status = AnalysisStatus.FramesCreated;
		await context.SaveChangesAsync(cancellationToken);
	}
}
