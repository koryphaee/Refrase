using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Refrase.Core;
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
	private readonly IOptions<RefraseOptions> options;
	private readonly DataPaths dataPaths;
	private TestDatabase database = null!;
	private FrameHashImporter importer = null!;

	public FrameHashImporterBenchmark()
	{
		options = InstanceFaker.FakeOptions();
		dataPaths = InstanceFaker.FakeDataPaths(options);
		File.Copy(new ResourcePaths().Video, dataPaths.Video(new VideoId(1)).Video, true);
	}

	[GlobalSetup]
	public async Task Setup()
	{
		database = await TestDatabase.Create();
		importer = new FrameHashImporter(NullLogger<FrameHashImporter>.Instance, database, dataPaths, new ImageHasher(), options);
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
