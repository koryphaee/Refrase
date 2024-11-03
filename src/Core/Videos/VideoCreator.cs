using Microsoft.EntityFrameworkCore.Storage;
using Refrase.Core.Analysis;
using Refrase.Core.Metadata;
using Refrase.Core.Paths;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Videos;

public class VideoCreator(
	RefraseContext context,
	DataPaths dataPaths,
	AnalysisQueue analysisQueue)
{
	public async Task<CreateVideoResponse> Create(CreateVideoRequest request, CancellationToken cancellationToken)
	{
		await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
		Video video = await CreateVideo(request, cancellationToken);
		await SaveFile(video.Id, request, cancellationToken);
		await transaction.CommitAsync(cancellationToken);
		await analysisQueue.Enqueue(video.Id, cancellationToken);
		return new CreateVideoResponse(video.Id);
	}

	private async Task<Video> CreateVideo(CreateVideoRequest request, CancellationToken cancellationToken)
	{
		Video video = new()
		{
			Name = request.Name,
			Category = request.Category,
			Url = request.Url,
			Imported = DateTime.UtcNow,
			Status = AnalysisStatus.Scheduled
		};
		context.Videos.Add(video);
		await context.SaveChangesAsync(cancellationToken);
		return video;
	}

	private async Task SaveFile(VideoId id, CreateVideoRequest request, CancellationToken cancellationToken)
	{
		string path = dataPaths.Video(id).Video;
		await using FileStream destination = File.OpenWrite(path);
		await request.Content.CopyToAsync(destination, cancellationToken);
	}
}
