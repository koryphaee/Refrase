using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refrase.Api.Frames.SearchFrame;
using Refrase.Api.Videos;
using Refrase.Api.Videos.IngestVideo;
using System.Diagnostics;

namespace Refrase.Api;

public static class Extensions
{
	public static void AddApi(this IServiceCollection services)
	{
		services
			.AddScoped<IngestVideoHandler>()
			.AddScoped<SearchFrameHandler>();
	}

	public static void MapApi(this WebApplication app)
	{
		app.MapLogging();
		app.MapEndpoints();
	}

	private static void MapLogging(this WebApplication app)
	{
		app.UseWhen(c => c.Request.Path.StartsWithSegments("/api"), builder =>
		{
			builder.Use(async (context, next) =>
			{
				long start = Stopwatch.GetTimestamp();
				await next(context);
				TimeSpan elapsed = Stopwatch.GetElapsedTime(start);
				app.Logger.LogInformation("{method} to {path} returned {statusCode} after {duration:F2} ms",
					context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed.TotalMilliseconds);
			});
		});
	}

	private static void MapEndpoints(this WebApplication app)
	{
		app.MapGet("/api/health", () => "OK");

		app.MapPost("/api/video", (IngestVideoHandler handler, [AsParameters] IngestVideoRequest request, CancellationToken cancellationToken) => handler.Handle(request, cancellationToken))
			.WithFormOptions(multipartBodyLengthLimit: 10L * 1024 * 1024 * 1024)
			.DisableAntiforgery();

		app.MapPost("/api/frame/search", (SearchFrameHandler handler, Stream body, CancellationToken cancellationToken) => handler.Handle(body, cancellationToken));
	}
}
