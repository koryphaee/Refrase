using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Model;
using Refrase.Model.Videos;
using System.Collections.Concurrent;

namespace Refrase.Core.Search;

public class FrameSearcher(
	ILogger<FrameSearcher> logger,
	HashComparer hashComparer,
	FrameCache frameCache,
	IOptions<RefraseOptions> options)
{
	public async Task<Match?> GetMatch(RefraseContext context, ulong hash, CancellationToken cancellationToken)
	{
		logger.LogInformation("Searching for frame with hash similar to {hash}", hash);
		FrameInfo? frame = await GetClosestFrame(hash, cancellationToken);

		if (frame is null)
			return null;

		Video? video = await context.Videos.Where(v => v.Id == frame.VideoId).SingleOrDefaultAsync(cancellationToken);

		if (video is null)
			return null;

		double similarity = hashComparer.Similarity(hash, frame.Hash);
		logger.LogInformation("Frame {frame} of video {video} has similarity {similarity} to {hash}", frame.Index, video.Name, similarity, hash);
		return new Match(video, frame, similarity);
	}

	private async Task<FrameInfo?> GetClosestFrame(ulong hash, CancellationToken cancellationToken)
	{
		IEnumerable<FrameInfo[]> chunks = await frameCache.GetAll(cancellationToken);
		ConcurrentBag<FrameInfo> frameInfos = [];

		var parallelOptions = new ParallelOptions
		{
			CancellationToken = cancellationToken,
			MaxDegreeOfParallelism = options.Value.SearchThreads ?? Environment.ProcessorCount
		};
		Parallel.ForEach(chunks, parallelOptions, chunk =>
		{
			FrameInfo? bestMatch = chunk.MinBy(v => hashComparer.HammingDistance(hash, v.Hash));
			if (bestMatch is not null)
				frameInfos.Add(bestMatch);
		});

		return frameInfos.MinBy(v => hashComparer.HammingDistance(hash, v.Hash));
	}
}
