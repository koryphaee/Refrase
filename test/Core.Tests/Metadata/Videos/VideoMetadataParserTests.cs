using FluentAssertions;
using NUnit.Framework;
using Refrase.Core.Metadata.Videos;

namespace Refrase.Core.Tests.Metadata.Videos;

public class VideoMetadataParserTests
{
	[Test]
	public void Test()
	{
		FfprobeStreamMetadata[] streams =
		[
			new(1920, 1080, "30000/1001", "1234", "123456.789")
		];
		FfprobeVideoMetadata video = new(streams);

		VideoMetadata metadata = new VideoMetadataParser().Parse(video);

		metadata.Width.Should().Be(1920);
		metadata.Height.Should().Be(1080);
		metadata.Duration.Should().BeCloseTo(new TimeSpan(1, 10, 17, 36, 789), TimeSpan.FromMilliseconds(1));
		metadata.FrameCount.Should().Be(1234);
		metadata.FrameRate.Should().Be(30);
	}
}
