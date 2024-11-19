using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Metadata.Videos;
using Refrase.Core.Paths;
using Refrase.Core.Tests.Helpers;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Tests.Metadata.Videos;

public class VideoMetadataImporterTests
{
	[Test]
	public async Task Test()
	{
		DataPaths dataPaths = InstanceFaker.FakeDataPaths();
		var parser = new VideoMetadataParser();
		var reader = new VideoMetadataReader(NullLogger<VideoMetadataReader>.Instance, parser);
		await using var database = await TestDatabase.Create();
		var importer = new VideoMetadataImporter(reader, database, dataPaths);

		await using RefraseContext context = database.CreateDbContext();
		var video = new Video
		{
			Name = "test",
			Category = "test",
			Imported = DateTime.Now,
		};
		EntityEntry<Video> entry = context.Add(video);
		await context.SaveChangesAsync();

		File.Copy(new ResourcePaths().Video, dataPaths.Video(video.Id).Video, true);

		await importer.Import(video.Id, CancellationToken.None);

		await entry.ReloadAsync();
		video.Width.Should().Be(1280);
		video.Height.Should().Be(720);
		video.Duration.Should().BeCloseTo(new TimeSpan(0, 0, 0, 10, 11), TimeSpan.FromMilliseconds(1));
		video.FrameCount.Should().Be(300);
		video.FrameRate.Should().Be(30);
		video.Status.Should().Be(AnalysisStatus.MetadataImported);
	}
}
