using Refrase.Model;
using System.Threading.Channels;

namespace Refrase.Core.Analysis;

public sealed class AnalysisQueue : IDisposable
{
	private readonly Channel<long> channel = Channel.CreateUnbounded<long>();

	public async Task Enqueue(long videoId, CancellationToken cancellationToken)
	{
		await channel.Writer.WriteAsync(videoId, cancellationToken);
	}

	public async Task<long> Dequeue(CancellationToken cancellationToken)
	{
		return await channel.Reader.ReadAsync(cancellationToken);
	}

	void IDisposable.Dispose()
	{
		channel.Writer.Complete();
	}
}
