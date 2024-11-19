using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Frames;
using Refrase.Core.Metadata.Frames;
using Refrase.Core.Paths;
using Refrase.Core.Tests.Helpers;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Tests.Metadata.Frames;

public class FrameMetadataImporterTests
{
	[Test]
	public async Task Test()
	{
		DataPaths dataPaths = InstanceFaker.FakeDataPaths();
		var parser = new FrameMetadataParser();
		var creator = new ExtraFrameCreator();
		var reader = new FrameMetadataReader(NullLogger<FrameMetadataReader>.Instance, parser, creator);
		await using var database = await TestDatabase.Create();
		var importer = new FrameMetadataImporter(reader, database, dataPaths);

		await using RefraseContext context = database.CreateDbContext();
		var video = new Video
		{
			Name = "test",
			Category = "test",
			Imported = DateTime.Now,
			Status = AnalysisStatus.MetadataImported
		};
		EntityEntry<Video> entry = context.Add(video);
		await context.SaveChangesAsync();

		File.Copy(new ResourcePaths().Video, dataPaths.Video(video.Id).Video, true);

		await importer.Import(video.Id, CancellationToken.None);

		Frame[] frames = await context.Frames
			.Where(f => f.VideoId == video.Id)
			.OrderBy(f => f.Index)
			.ToArrayAsync();

		frames.Should().HaveCount(300);
		frames.Select(f => f.Index).Should().BeInAscendingOrder();
		frames.Select(f => f.Timestamp).Should().BeInAscendingOrder();

		await entry.ReloadAsync();
		video.Status.Should().Be(AnalysisStatus.FramesCreated);
	}
}
