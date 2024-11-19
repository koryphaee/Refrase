using Microsoft.Extensions.Options;
using Refrase.Core.Paths;

namespace Refrase.Core.Tests.Helpers;

public static class PathFaker
{
	public static DataPaths Fake()
	{
		var refraseOptions = new RefraseOptions
		{
			DataDirectory = "."
		};
		IOptions<RefraseOptions> options = new OptionsWrapper<RefraseOptions>(refraseOptions);
		var mappedPaths = new MappedPaths(options);
		return new DataPaths(mappedPaths);
	}
}
