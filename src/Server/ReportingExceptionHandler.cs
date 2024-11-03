using Microsoft.AspNetCore.Diagnostics;
using Refrase.Core.Reporting;

namespace Refrase.Server;

public class ReportingExceptionHandler(
	ErrorNotifier errorNotifier)
	: IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		await errorNotifier.Notify(exception, cancellationToken);
		return false;
	}
}
