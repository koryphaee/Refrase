using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Metadata.Videos;
using Refrase.Core.Tests.Helpers;

namespace Refrase.Core.Tests.Metadata.Videos;

public class VideoMetadataReaderTests
{
	[Test]
	public async Task Test()
	{
		string path = new ResourcePaths().Video;
		var extractor = new VideoMetadataReader(NullLogger<VideoMetadataReader>.Instance, new VideoMetadataParser());
		VideoMetadata metadata = await extractor.ExtractMetadata(path, CancellationToken.None);

		metadata.Width.Should().Be(1280);
		metadata.Height.Should().Be(720);
		metadata.Duration.Should().BeCloseTo(new TimeSpan(0, 0, 0, 10, 11), TimeSpan.FromMilliseconds(1));
		metadata.FrameCount.Should().Be(300);
		metadata.FrameRate.Should().Be(30);
	}
}
