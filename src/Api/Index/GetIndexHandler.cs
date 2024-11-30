using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Refrase.Core.Analysis;
using Refrase.Model;

namespace Refrase.Api.Index;

public class GetIndexHandler(
	RefraseContext context,
	AnalysisQueue analysisQueue)
{
	public async Task<Ok<IndexDto>> Handle(CancellationToken cancellationToken)
	{
		int videoCount = await context.Videos.CountAsync(cancellationToken);
		int frameCount = await context.Frames.CountAsync(cancellationToken);
		DateTime lastUpdate = await context.Videos.Where(v => v.Analyzed != null).MaxAsync(v => v.Analyzed, cancellationToken) ?? new DateTime(1970, 1, 1);
		var indexDto = new IndexDto(videoCount, frameCount, analysisQueue.Length, lastUpdate);
		return TypedResults.Ok(indexDto);
	}
}
