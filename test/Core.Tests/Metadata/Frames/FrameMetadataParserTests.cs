using FluentAssertions;
using NUnit.Framework;
using Refrase.Core.Metadata.Frames;

namespace Refrase.Core.Tests.Metadata.Frames;

public class FrameMetadataParserTests
{
	[Test]
	public void Test()
	{
		FfprobeFrameMetadata[] streams =
		[
			new("123.4"),
			new("567.89")
		];
		FfprobeFramesMetadata frames = new(streams);

		FrameMetadata[] metadata = new FrameMetadataParser().Parse(frames);

		metadata.Should().HaveCount(2);
		metadata[0].Index.Should().Be(0);
		metadata[0].Timestamp.Should().BeCloseTo(new TimeSpan(0, 0, 2, 3, 400), TimeSpan.FromMilliseconds(1));
		metadata[1].Index.Should().Be(1);
		metadata[1].Timestamp.Should().BeCloseTo(new TimeSpan(0, 0, 9, 27, 890), TimeSpan.FromMilliseconds(1));
	}
}
