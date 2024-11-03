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
public class FrameHashImporterBenchmark
{
	private readonly DataPaths dataPaths;
	private TestDatabase database = null!;
	private FrameHashImporter importer = null!;

	public FrameHashImporterBenchmark()
	{
		dataPaths = PathFaker.Fake();
		File.Copy(new ResourcePaths().Video, dataPaths.Video(new VideoId(1)).Video, true);
	}

	[GlobalSetup]
	public async Task Setup()
	{
		database = await TestDatabase.Create();
		importer = new FrameHashImporter(NullLogger<FrameHashImporter>.Instance, database, dataPaths, new ImageHasher());
	}

	[IterationSetup]
	public async Task IterationSetup()
	{
		await using RefraseContext context = database.CreateDbContext();
		await context.Frames.ExecuteUpdateAsync(f => f.SetProperty(x => x.Hash, x => null));
		await context.Videos.ExecuteUpdateAsync(f => f.SetProperty(x => x.Status, x => AnalysisStatus.FramesCreated));
	}

	[Benchmark]
	public async Task Serial()
	{
		await importer.Import(new VideoId(1), default);
	}
}
