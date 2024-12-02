using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Core.Search;
using Refrase.Core.Tests.Helpers;
using Refrase.Model;
using Refrase.Model.Frames;
using Refrase.Model.Videos;

namespace Refrase.Core.Tests.Search;

public class FrameSearcherTests
{
	[Test]
	public async Task Test()
	{
		var hashComparer = new HashComparer(NullLogger<HashComparer>.Instance);
		await using TestDatabase database = await TestDatabase.Create();
		var frameCache = new FrameCache(database, NullLogger<FrameCache>.Instance);
		var frameSearcher = new FrameSearcher(NullLogger<FrameSearcher>.Instance, hashComparer, frameCache, new OptionsWrapper<RefraseOptions>(new RefraseOptions()));

		var video = new Video
		{
			Name = "Test",
			Category = "Test",
			Imported = DateTime.Now,
			Frames =
			[
				new Frame
				{
					Index = 0,
					Timestamp = TimeSpan.FromSeconds(1),
					Hash = 67890
				},
				new Frame
				{
					Index = 1,
					Timestamp = TimeSpan.FromSeconds(2),
					Hash = 12344
				},
				new Frame
				{
					Index = 2,
					Timestamp = TimeSpan.FromSeconds(3),
					Hash = 1234567890
				}
			]
		};

		await using RefraseContext context = database.CreateDbContext();
		context.Add(video);
		await context.SaveChangesAsync();

		Match? match = await frameSearcher.GetMatch(context, 12345, default);

		match.Should().NotBeNull();
		match!.Similarity.Should().Be(0.984375);
		match.Video.Id.Should().Be(video.Id);
		match.Frame.Index.Should().Be(video.Frames.ElementAt(1).Index);
	}
}
