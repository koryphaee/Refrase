using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using Refrase.Core.Hashing;

namespace Refrase.Benchmarks;

[InProcess]
public class HashComparerBenchmarks
{
	private readonly ulong a = (ulong) Random.Shared.NextInt64();
	private readonly ulong b = (ulong) Random.Shared.NextInt64();
	private readonly HashComparer hashComparer = new(NullLogger<HashComparer>.Instance);

	[Benchmark]
	public int HammingDistance()
	{
		return hashComparer.HammingDistance(a, b);
	}
}
