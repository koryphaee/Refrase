using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Refrase.Core.Videos;
using Refrase.Model;

namespace Refrase.Api.Videos.IngestVideo;

public class IngestVideoHandler(
	RefraseContext context,
	VideoCreator videoCreator,
	VideoSaver videoSaver,
	VideoValidator videoValidator)
{
	public async Task<Results<Conflict, BadRequest, Ok<IngestVideoResponse>>> Handle(
		IngestVideoRequest request, CancellationToken cancellationToken)
	{
		bool duplicate = await context.Videos.AnyAsync(v => v.Name == request.Name, cancellationToken);

		if (duplicate)
			return TypedResults.Conflict();

		await using Stream stream = request.Video.OpenReadStream();
		string path = await videoSaver.Save(stream, cancellationToken);
		bool valid = await videoValidator.Validate(path, cancellationToken);

		if (!valid)
			return TypedResults.BadRequest();

		try
		{
			var video = new CreateVideoRequest(request.Name, request.Category, request.Url, path);
			CreateVideoResponse response = await videoCreator.Create(video, cancellationToken);
			return TypedResults.Ok(new IngestVideoResponse(response.Id));
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
