using MediatR;

namespace SamaniCrm.Application.Auth.Events;

public class UserLogedInNotificationHandler : INotificationHandler<UserLogedInEvent>
{
    public Task Handle(UserLogedInEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User: {notification.userName} = {notification.message}");
        return Task.CompletedTask;
    }
}