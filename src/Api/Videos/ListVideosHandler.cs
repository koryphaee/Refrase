using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Refrase.Api.Frames.SearchFrame;
using Refrase.Model;

namespace Refrase.Api.Videos;

public class ListVideosHandler(RefraseContext context)
{
	public async Task<Ok<VideoDto[]>> Handle(CancellationToken cancellationToken)
	{
		VideoDto[] videos = await context.Videos
			.Select(v => new VideoDto(v.Id, v.Name, v.Category, v.Url))
			.ToArrayAsync(cancellationToken);
		return TypedResults.Ok(videos);
	}
}
