using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Refrase.Core.Reporting;

public class ErrorNotifier(
	IOptionsMonitor<RefraseOptions> options,
	ILogger<ErrorNotifier> logger)
{
    public async Task Notify(Exception exception, CancellationToken cancellationToken)
    {
	    RefraseOptions value = options.CurrentValue;

	    if (value.PushoverToken is not string token)
        {
            logger.LogWarning("Unable to send notification because no token was provided");
            return;
        }

        if (value.PushoverUser is not string user)
        {
            logger.LogWarning("Unable to send notification because no user was provided");
            return;
        }

        try
        {
	        string message = exception.ToString();
            await "https://api.pushover.net/1/messages.json"
                .AppendQueryParam("token", token)
                .AppendQueryParam("user", user)
                .AppendQueryParam("message", message)
                .AppendQueryParam("title", "Refrase")
                .PostAsync(cancellationToken: cancellationToken);
            logger.LogInformation("Sent message '{message}' to Pushover", message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending notification");
        }
    }
}
