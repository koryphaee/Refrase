using Microsoft.Extensions.Configuration;
using Refrase.Core.Paths;

namespace Refrase.Core.Tests.Helpers;

public static class PathFaker
{
	public static DataPaths Fake()
	{
		Dictionary<string, string?> values = new()
		{
			{ Constants.DataDirectory, "." }
		};

		IConfigurationRoot configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(values)
			.Build();

		var mappedPaths = new MappedPaths(configuration);
		return new DataPaths(mappedPaths);
	}
}
