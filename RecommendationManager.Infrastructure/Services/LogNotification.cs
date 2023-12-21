using Microsoft.Extensions.Logging;
using RecommendationManager.Application.Interfaces;

namespace RecommendationManager.Infrastructure.Services;

public class LogNotification : INotificationService
{
    private readonly ILogger<LogNotification> _logger;

    public LogNotification(ILogger<LogNotification> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string message)
    {
        _logger.LogInformation("notification: {msg}", message);
        return Task.CompletedTask;
    }
}
