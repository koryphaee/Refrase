using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Refrase.Core.Hashing;
using Refrase.Core.Paths;
using Refrase.Core.Search;
using Refrase.Model;

namespace Refrase.Api.Frames.SearchFrame;

public class SearchFrameHandler(
	ILogger<SearchFrameHandler> logger,
	ImageHasher imageHasher,
	FrameSearcher frameSearcher,
	RefraseContext context,
	DataPaths dataPaths)
{
	public async Task<Results<Ok<SearchFrameResponse>, BadRequest>> Handle(Stream body, CancellationToken cancellationToken)
	{
		string file = await SaveFile(body, cancellationToken);

		try
		{
			ulong hash = await imageHasher.HashImage(file);
			MatchDto? match = await GetMatch(hash, cancellationToken);
			SearchFrameResponse response = new(match, hash);
			return TypedResults.Ok(response);
		}
		finally
		{
			File.Delete(file);
		}
	}

	private async Task<string> SaveFile(Stream body, CancellationToken cancellationToken)
	{
		Guid guid = Guid.NewGuid();
		string path = dataPaths.Frame.Image(guid);
		logger.LogDebug("Writing image to file {path}", path);
		await using FileStream target = File.OpenWrite(path);
		await body.CopyToAsync(target, cancellationToken);
		return path;
	}

	private async Task<MatchDto?> GetMatch(ulong hash, CancellationToken cancellationToken)
	{
		Match? match = await frameSearcher.GetMatch(context, hash, cancellationToken);

		if (match is null)
			return null;

		VideoDto video = new(match.Video.Name, match.Video.Category, match.Video.Url);
		FrameDto frame = new(match.Frame.Index, match.Frame.Timestamp, match.Frame.Hash, match.Similarity);
		return new MatchDto(video, frame);
	}
}
