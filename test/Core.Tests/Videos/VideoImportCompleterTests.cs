using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;
using Refrase.Core.Paths;
using Refrase.Core.Tests.Helpers;
using Refrase.Core.Videos;
using Refrase.Model;
using Refrase.Model.Videos;

namespace Refrase.Core.Tests.Videos;

public class VideoImportCompleterTests
{
	[Test]
	public async Task Test()
	{
		await using TestDatabase database = await TestDatabase.Create();
		DataPaths dataPaths = InstanceFaker.FakeDataPaths();
		var completer = new VideoImportCompleter(database, dataPaths);

		await using RefraseContext context = database.CreateDbContext();
		var video = new Video
		{
			Name = "test",
			Category = "test",
			Imported = DateTime.Now,
			Status = AnalysisStatus.FramesHashed
		};
		EntityEntry<Video> entry = context.Add(video);
		await context.SaveChangesAsync();

		File.Copy(new ResourcePaths().Video, dataPaths.Video(video.Id).ReEncodedVideo, true);
		for (int i = 0; i < 3; i++)
		{
			File.Copy(new ResourcePaths().Frame(i), dataPaths.Video(video.Id).Frames.Frame(i), true);
		}
		await File.WriteAllTextAsync(dataPaths.Video(video.Id).FrameList, "frame list");

		await completer.Complete(video.Id, CancellationToken.None);

		await entry.ReloadAsync();
		video.Status.Should().Be(AnalysisStatus.Completed);
		Directory.GetFileSystemEntries(dataPaths.Video(video.Id).Directory).Should().BeEmpty();
	}
}
