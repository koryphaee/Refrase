using Refrase.Model;
using System.Threading.Channels;

namespace Refrase.Core.Analysis;

public sealed class AnalysisQueue : IDisposable
{
	private readonly Channel<VideoId> channel = Channel.CreateUnbounded<VideoId>();

	public async Task Enqueue(VideoId videoId, CancellationToken cancellationToken)
	{
		await channel.Writer.WriteAsync(videoId, cancellationToken);
	}

	public async Task<VideoId> Dequeue(CancellationToken cancellationToken)
	{
		return await channel.Reader.ReadAsync(cancellationToken);
	}

	void IDisposable.Dispose()
	{
		channel.Writer.Complete();
	}
}
