using Microsoft.AspNetCore.SignalR;
using UnitedPayment.Model;

namespace UnitedPayment.Hubs
{
    public class ChatHub(AppDbContext context) : Hub
    {
        public static Dictionary<string, int> Employees= new();
        public async Task Connect(int userId)
        {
            Employees.Add(Context.ConnectionId, userId);
            Employee? employee = await context.Employees.FindAsync(userId);
            if (employee is not null)
            {
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", employee);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId;
            Employees.TryGetValue(Context.ConnectionId, out userId);
            Employees.Remove(Context.ConnectionId);
            Employee? user = await context.Employees.FindAsync(userId);
            if (user is not null)
            {
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }
    }
}
