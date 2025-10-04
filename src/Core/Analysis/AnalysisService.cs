using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refrase.Core.Reporting;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Analysis;

internal class AnalysisService(
	ILogger<AnalysisService> logger,
	IDbContextFactory<RefraseContext> contextFactory,
	AnalysisQueue analysisQueue,
	IServiceProvider serviceProvider,
	ErrorNotifier errorNotifier)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await EnqueueScheduled(stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			long videoId = await analysisQueue.Dequeue(stoppingToken);

			try
			{
				await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
				AnalysisExecutor executor = scope.ServiceProvider.GetRequiredService<AnalysisExecutor>();
				await executor.Execute(videoId, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception e)
			{
				logger.LogError(e, "Failed to import video {id}", videoId);
				await errorNotifier.Notify(e, stoppingToken);
			}
		}
	}

	private async Task EnqueueScheduled(CancellationToken cancellationToken)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
		long[] videoIds = await context.Videos
			.Where(v => v.Status != AnalysisStatus.Completed)
			.Select(v => v.Id)
			.ToArrayAsync(cancellationToken);

		foreach (long videoId in videoIds)
		{
			await analysisQueue.Enqueue(videoId, cancellationToken);
		}
		logger.LogInformation("Enqueued {count} videos for analysis", videoIds.Length);
	}
}
