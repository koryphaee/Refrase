using Microsoft.Extensions.Options;
using Refrase.Core.Paths;

namespace Refrase.Core.Tests.Helpers;

public static class InstanceFaker
{
	public static IOptions<RefraseOptions> FakeOptions()
	{
		var refraseOptions = new RefraseOptions
		{
			DataDirectory = "."
		};
		return new OptionsWrapper<RefraseOptions>(refraseOptions);
	}

	public static DataPaths FakeDataPaths(IOptions<RefraseOptions>? options = null)
	{
		options ??= FakeOptions();
		var mappedPaths = new MappedPaths(options);
		return new DataPaths(mappedPaths);
	}
}
