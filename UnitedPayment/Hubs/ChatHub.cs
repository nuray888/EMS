using Microsoft.AspNetCore.SignalR;
using UnitedPayment.Model;

namespace UnitedPayment.Hubs
{
    public class ChatHub(AppDbContext context) : Hub
    {
        public static Dictionary<string, int> Users = new();
        public async Task Connect(int userId)
        {
            Users.Add(Context.ConnectionId, userId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId;
            Users.TryGetValue(Context.ConnectionId, out userId);
            Users.Remove(Context.ConnectionId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }
    }
}
