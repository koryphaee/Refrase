using Microsoft.Extensions.Configuration;

namespace Refrase.Core.Paths;

public class MappedPaths(IConfiguration configuration)
{
	public string Data => configuration[Constants.DataDirectory] ?? throw new InvalidOperationException("No data path mapped!");
}
