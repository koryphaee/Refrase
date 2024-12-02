using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Refrase.Core.Hashing;

public class HashComparer(
	ILogger<HashComparer> logger)
{
	public int HammingDistance(ulong a, ulong b)
	{
		return BitOperations.PopCount(a ^ b);
	}

	public double Similarity(ulong a, ulong b)
	{
		const int length = 64;
		int distance = HammingDistance(a, b);
		double inverted = length - distance;
		double percentage = inverted / length;
		logger.LogTrace("Similarity between {a} and {b} is {similarity}", a, b, percentage);
		return percentage;
	}
}
