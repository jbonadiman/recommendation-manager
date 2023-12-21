namespace RecommendationManager.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(string message);
}
