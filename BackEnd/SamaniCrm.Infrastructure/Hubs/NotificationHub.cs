using Microsoft.AspNetCore.SignalR;

namespace SamaniCrm.Infrastructure.Hubs
{
    public class NotificationHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            Console.WriteLine($"User {userId} connected to SignalR hub.");
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            var username = Context.User?.Identity?.Name;
            await Clients.All.SendAsync("ReceiveMessage", $"{username}: {message}");
        }

    }
}
