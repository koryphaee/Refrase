using CliWrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refrase.Core.Hashing;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Frames;

public class FrameHashImporter(
	ILogger<FrameHashImporter> logger,
	IDbContextFactory<RefraseContext> contextFactory,
	DataPaths dataPaths,
	ImageHasher imageHasher,
	IOptions<RefraseOptions> options)
{
	public async Task Import(VideoId videoId, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		Video video = await context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

		if (video.Status != AnalysisStatus.FramesCreated)
			return;

		Frame[] frames = await context.Frames
			.AsNoTracking()
			.Where(f => f.VideoId == videoId)
			.OrderBy(f => f.Index)
			.ToArrayAsync(cancellationToken);

		foreach (Frame[] chunk in frames.Chunk(100))
		{
			if (chunk.All(f => f.Hash.HasValue))
				continue;

			await ImportChunk(chunk, cancellationToken);
		}

		video.Status = AnalysisStatus.FramesHashed;
		await context.SaveChangesAsync(cancellationToken);
	}

	private async Task ImportChunk(Frame[] frames, CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		context.Frames.AttachRange(frames);
		Frame first = frames.First();
		VideoId videoId = first.VideoId;
		string videoPath = dataPaths.Video(videoId).Video;
		string framePattern = dataPaths.Video(videoId).Frames.Pattern;

		await Cli
			.Wrap("ffmpeg")
			.WithArguments(a => a
				.Add("-ss").Add(first.Timestamp.ToString("c"))
				.Add("-i").Add(videoPath)
				.Add("-start_number").Add(first.Index)
				.Add("-vframes").Add(frames.Length)
				.Add("-threads").Add(options.Value.FfmpegThreads)
				.Add(framePattern))
			.Run(logger, cancellationToken);

		foreach (Frame frame in frames)
		{
			string framePath = dataPaths.Video(videoId).Frames.Frame(frame.Index);
			frame.Hash ??= await imageHasher.HashImage(framePath);
		}

		await context.SaveChangesAsync(cancellationToken);
	}
}
