using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Refrase.Core.Reporting;

public class ErrorNotifier(
	IConfiguration configuration,
	ILogger<ErrorNotifier> logger)
{
    public async Task Notify(Exception exception, CancellationToken cancellationToken)
    {
        if (configuration["PushoverToken"] is not string token)
        {
            logger.LogWarning("Unable to send notification because no token was provided");
            return;
        }

        if (configuration["PushoverUser"] is not string user)
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
