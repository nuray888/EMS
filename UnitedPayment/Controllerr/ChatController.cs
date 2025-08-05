using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UnitedPayment.Hubs;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs.Requests;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(
        AppDbContext context,
        IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> getChats(int userId, int toUserId, CancellationToken cancellationToken)
        {
            List<Chat> chats = await context.Chats.Where(p => p.UserId == userId && p.toUserId == toUserId
            || p.UserId == p.toUserId && p.toUserId == p.UserId).OrderBy(p => p.Date).ToListAsync(cancellationToken);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequestDTO request, CancellationToken cancellationToken)
        {
            Chat chat = new()
            {
                UserId = request.userId,
                toUserId = request.toUserId,
                Message = request.Message,
                Date = DateTime.Now
            };
            await context.AddAsync(chat, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            string connectionId = ChatHub.Users.First(p => p.Value == chat.toUserId).Key;

            await hubContext.Clients.Client(connectionId).SendAsync("Messages", chat);

            return Ok(chat);
        }

    }
}
