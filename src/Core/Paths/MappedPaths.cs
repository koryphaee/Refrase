using Microsoft.Extensions.Options;

namespace Refrase.Core.Paths;

public class MappedPaths(IOptions<RefraseOptions> options)
{
	public string Data => options.Value.DataDirectory ?? throw new InvalidOperationException("No data path mapped!");
}
