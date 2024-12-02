using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refrase.Core.Analysis;
using Refrase.Core.Frames;
using Refrase.Core.Hashing;
using Refrase.Core.Metadata.Frames;
using Refrase.Core.Metadata.Videos;
using Refrase.Core.Paths;
using Refrase.Core.Reporting;
using Refrase.Core.Search;
using Refrase.Core.Videos;
using System.Globalization;

namespace Refrase.Core;

public static class Extensions
{
	public static void AddCore(this IServiceCollection services)
	{
		services
			.AddHostedService<AnalysisService>()
			.AddSingleton<AnalysisQueue>()
			.AddSingleton<AnalysisExecutor>();

		services
			.AddSingleton<FrameCache>()
			.AddSingleton<FrameHashImporter>();

		services
			.AddSingleton<HashComparer>()
			.AddSingleton<ImageHasher>();

		services
			.AddSingleton<FrameMetadataParser>()
			.AddSingleton<FrameMetadataReader>()
			.AddSingleton<FrameMetadataImporter>();

		services
			.AddSingleton<VideoMetadataParser>()
			.AddSingleton<VideoMetadataReader>()
			.AddSingleton<VideoMetadataImporter>();

		services
			.AddSingleton<DataPaths>()
			.AddSingleton<MappedPaths>();

		services
			.AddSingleton<FrameSearcher>();

		services
			.AddScoped<VideoCreator>()
			.AddSingleton<VideoImportCompleter>()
			.AddSingleton<VideoSaver>()
			.AddSingleton<VideoReEncoder>();

		services
			.AddSingleton<ErrorNotifier>();
	}

	internal static async Task<BufferedCommandResult> Run(this Command command, ILogger logger, CancellationToken cancellationToken)
	{
		BufferedCommandResult result = await command
			.WithValidation(CommandResultValidation.None)
			.ExecuteBufferedAsync(cancellationToken);

		if (result.IsSuccess)
		{
			logger.LogDebug("Command '{command}' finished successfully after {time}", command, result.RunTime);
			return result;
		}

		logger.LogError("Command '{command}' failed exit code {code} after {time} and printed '{output}'", command, result.ExitCode, result.RunTime, result.StandardError);
		throw new RefraseException($"Command {command} failed with exit code {result.ExitCode}");
	}

	internal static T Parse<T>(this string s, string name)
		where T : struct, IParsable<T>
	{
		return T.TryParse(s, CultureInfo.InvariantCulture, out T result)
			? result
			: throw new RefraseException($"Invalid {name} '{s}'");
	}
}
