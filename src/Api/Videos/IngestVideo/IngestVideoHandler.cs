using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Refrase.Core.Videos;
using Refrase.Model;

namespace Refrase.Api.Videos.IngestVideo;

public class IngestVideoHandler(
	RefraseContext context,
	VideoCreator videoCreator)
{
	public async Task<Results<Conflict, Ok<IngestVideoResponse>>> Handle(IngestVideoRequest request, CancellationToken cancellationToken)
	{
		bool duplicate = await context.Videos.AnyAsync(v => v.Name == request.Name, cancellationToken);

		if (duplicate)
			return TypedResults.Conflict();

		await using Stream content = request.Video.OpenReadStream();
		var video = new CreateVideoRequest(request.Name, request.Category, request.Url, content);
		CreateVideoResponse response = await videoCreator.Create(video, cancellationToken).ConfigureAwait(false);
		return TypedResults.Ok(new IngestVideoResponse(response.Id));
	}
}
