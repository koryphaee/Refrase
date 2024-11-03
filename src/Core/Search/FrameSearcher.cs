using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Search;

public class FrameSearcher(
	ILogger<FrameSearcher> logger,
	HashComparer hashComparer,
	FrameCache frameCache)
{
	public async Task<Match?> GetMatch(RefraseContext context, ulong hash, CancellationToken cancellationToken)
	{
		logger.LogInformation("Searching for frame with hash similar to {hash}", hash);
		FrameInfo[] frames = await frameCache.GetAll(cancellationToken);
		FrameInfo? frame = frames.MinBy(v => hashComparer.HammingDistance(hash, v.Hash));

		if (frame is null)
			return null;

		Video? video = await context.Videos.Where(v => v.Id == frame.VideoId).SingleOrDefaultAsync(cancellationToken);

		if (video is null)
			return null;

		double similarity = hashComparer.Similarity(hash, frame.Hash);
		logger.LogInformation("Frame {frame} of video {video} has similarity {similarity} to {hash}", frame.Index, video.Name, similarity, hash);
		return new Match(video, frame, similarity);
	}
}
