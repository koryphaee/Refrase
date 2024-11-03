using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Core.Paths;
using Refrase.Core.Tests.Helpers;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Tests.Frames;

public class FrameHashImporterTests
{
	[Test]
	public async Task Test()
	{
		await using var database = await TestDatabase.Create();
		VideoId videoId = await CreateData(database);

		DataPaths dataPaths = PathFaker.Fake();
		File.Copy(new ResourcePaths().Video, dataPaths.Video(videoId).Video, true);

		var imageHasher = new ImageHasher();
		var importer = new FrameHashImporter(NullLogger<FrameHashImporter>.Instance, database, dataPaths, imageHasher);

		await importer.Import(videoId, default);

		await using RefraseContext context = database.CreateDbContext();

		Video video = await context.Videos
			.Include(v => v.Frames)
			.Where(v => v.Id == videoId)
			.SingleAsync();
		video.Status.Should().Be(AnalysisStatus.FramesHashed);
		video.Frames.Should().AllSatisfy(f => f.Hash.Should().NotBeNull());
	}

	private static async Task<VideoId> CreateData(IDbContextFactory<RefraseContext> contextFactory)
	{
		await using RefraseContext context = await contextFactory.CreateDbContextAsync();

		var video = new Video
		{
			Name = "test",
			Category = "test",
			Imported = DateTime.Now,
			Status = AnalysisStatus.FramesCreated
		};

		for (int i = 0; i < 300; i++)
		{
			var frame = new Frame
			{
				Video = video,
				Index = i,
				Timestamp = TimeSpan.FromMilliseconds(33 * i)
			};
			context.Add(frame);
		}

		await context.SaveChangesAsync();
		return video.Id;
	}
}
