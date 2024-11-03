using FluentAssertions;
using NUnit.Framework;
using Refrase.Core.Hashing;
using Refrase.Core.Tests.Helpers;

namespace Refrase.Core.Tests.Hashing;

public class ImageHasherTests
{
	[Test]
	public async Task Test()
	{
		string path = new ResourcePaths().Frame(0);
		ulong hash = await new ImageHasher().HashImage(path);
		hash.Should().Be(15770234999689110752);
	}
}
