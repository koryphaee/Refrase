using Microsoft.Extensions.Logging;
using Refrase.Core.Frames;
using Refrase.Core.Metadata.Frames;
using Refrase.Core.Metadata.Videos;
using Refrase.Core.Videos;
using Refrase.Model;

namespace Refrase.Core.Analysis;

internal class AnalysisExecutor(
	ILogger<AnalysisExecutor> logger,
	VideoReEncoder videoReEncoder,
	VideoMetadataImporter videoMetadataImporter,
	FrameMetadataImporter frameMetadataImporter,
	FrameHashImporter frameHashImporter,
	VideoImportCompleter videoImportCompleter,
	FrameCache frameCache)
{
	public async Task Execute(long videoId, CancellationToken cancellationToken)
	{
		logger.LogInformation("Analyzing video {videoId}", videoId);
		await videoReEncoder.ReEncode(videoId, cancellationToken);
		await videoMetadataImporter.Import(videoId, cancellationToken);
		await frameMetadataImporter.Import(videoId, cancellationToken);
		await frameHashImporter.Import(videoId, cancellationToken);
		await videoImportCompleter.Complete(videoId, cancellationToken);
		logger.LogInformation("Successfully finished analysis of video {videoId}", videoId);
		await frameCache.Update(videoId, cancellationToken);
	}
}
