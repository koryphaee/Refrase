using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Refrase.Core.Hashing;

namespace Refrase.Core.Tests.Hashing;

public class HashComparerTests
{
	[Test]
	public void HammingDistance()
	{
		var calculator = new HashComparer(NullLogger<HashComparer>.Instance);
		int distance = calculator.HammingDistance(12345, 67890);
		distance.Should().Be(8);
	}

	[Test]
	public void Similarity()
	{
		var calculator = new HashComparer(NullLogger<HashComparer>.Instance);
		double similarity = calculator.Similarity(12345, 67890);
		similarity.Should().Be(0.875);
	}
}
