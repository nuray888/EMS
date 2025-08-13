using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HrManagement.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var role = Context.User?.IsInRole("Admin") == true ? "admin" :
                       Context.User?.IsInRole("DepartmentHead") == true ? "rəhbər" : "işçi";

            await Clients.All.SendAsync("ReceiveMessage", new { userId, role, message });
        }
    }
}