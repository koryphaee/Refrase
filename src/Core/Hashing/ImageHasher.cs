using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;

namespace Refrase.Core.Hashing;

public class ImageHasher
{
	private readonly PerceptualHash hash = new();

	public async Task<ulong> HashImage(string imagePath)
	{
		await using FileStream stream = File.OpenRead(imagePath);
		return hash.Hash(stream);
	}
}
