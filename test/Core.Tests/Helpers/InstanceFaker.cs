using Microsoft.Extensions.Options;
using Refrase.Core.Paths;

namespace Refrase.Core.Tests.Helpers;

public static class InstanceFaker
{
	private class FakeOptionsSnapshot<T>(T value) : IOptionsSnapshot<T>
		where T : class
	{
		public T Value { get; } = value;

		public T Get(string? name)
		{
			return Value;
		}
	}

	public static IOptionsSnapshot<RefraseOptions> FakeOptions()
	{
		var refraseOptions = new RefraseOptions
		{
			DataDirectory = "."
		};
		return new FakeOptionsSnapshot<RefraseOptions>(refraseOptions);
	}

	public static DataPaths FakeDataPaths(IOptions<RefraseOptions>? options = null)
	{
		options ??= FakeOptions();
		var mappedPaths = new MappedPaths(options);
		return new DataPaths(mappedPaths);
	}
}
