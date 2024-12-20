﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refrase.Model;
using Refrase.Model.Frames;

namespace Refrase.Core.Frames;

public class FrameCache(
	IDbContextFactory<RefraseContext> contextFactory,
	ILogger<FrameCache> logger)
{
	private readonly SemaphoreSlim semaphore = new(1);
	private readonly Dictionary<long, FrameInfo[]> cache = [];
	private bool loaded;

	public async Task Update(long videoId, CancellationToken cancellationToken)
	{
		await semaphore.WaitAsync(cancellationToken);

		try
		{
			logger.LogDebug("Updating frame cache of video {videoId}", videoId);
			await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
			Frame[] frames = await context.Frames
				.Where(f => f.VideoId == videoId)
				.Where(f => f.Hash.HasValue)
				.ToArrayAsync(cancellationToken);
			cache[videoId] = frames.Select(Convert).ToArray();
		}
		finally
		{
			semaphore.Release();
		}
	}

	public async Task<IEnumerable<FrameInfo[]>> GetAll(CancellationToken cancellationToken)
	{
		await semaphore.WaitAsync(cancellationToken);

		try
		{
			await EnsureLoaded(cancellationToken);
			return cache.Values;
		}
		finally
		{
			semaphore.Release();
		}
	}

	private async Task EnsureLoaded(CancellationToken cancellationToken)
	{
		if (loaded)
			return;

		try
		{
			await using RefraseContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
			Frame[] frames = await context.Frames
				.Where(f => f.Hash.HasValue)
				.ToArrayAsync(cancellationToken);

			ILookup<long, FrameInfo> lookup = frames.ToLookup(f => f.VideoId, Convert);
			foreach (IGrouping<long, FrameInfo> grouping in lookup)
			{
				cache[grouping.Key] = grouping.ToArray();
			}
			loaded = true;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error loading frames");
		}
	}

	private static FrameInfo Convert(Frame frame)
	{
		return new FrameInfo(
			frame.VideoId,
			frame.Index,
			frame.Timestamp,
			frame.Hash!.Value);
	}
}
