using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Core.Paths;
using Refrase.Core.Tests.Helpers;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Benchmarks;

[InProcess]
public class ImageHasherBenchmark
{
	private readonly ResourcePaths resourcePaths = new();
	private readonly ImageHasher imageHasher = new();

	[Benchmark]
	public async Task Serial()
	{
		for (int i = 0; i < 10; i++)
		{
			await imageHasher.HashImage(resourcePaths.Frame(i));
		}
	}

	[Benchmark]
	public async Task Parallel()
	{
		await System.Threading.Tasks.Parallel.ForAsync(0, 10, async (i, _) => await imageHasher.HashImage(resourcePaths.Frame(i)));
	}
}
