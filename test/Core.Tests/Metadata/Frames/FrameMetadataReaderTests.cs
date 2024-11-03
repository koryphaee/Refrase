using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Frames;
using Refrase.Core.Metadata.Frames;
using Refrase.Core.Metadata.Videos;
using Refrase.Core.Tests.Helpers;

namespace Refrase.Core.Tests.Metadata.Frames;

public class FrameMetadataReaderTests
{
	[Test]
	public async Task Test()
	{
		string path = new ResourcePaths().Video;
		var extractor = new FrameMetadataReader(NullLogger<FrameMetadataReader>.Instance, new FrameMetadataParser(), new ExtraFrameCreator());
		FrameMetadata[] metadata = await extractor.ExtractMetadata(path, CancellationToken.None);

		metadata.Should().HaveCount(300);

		for (int i = 0; i < metadata.Length; i++)
		{
			metadata[i].Index.Should().Be(i);
			metadata[i].Timestamp.Should().BeCloseTo(TimeSpan.FromMilliseconds(33 * i), TimeSpan.FromMilliseconds(i));
		}
	}
}
