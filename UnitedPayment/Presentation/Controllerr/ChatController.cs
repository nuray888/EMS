using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using UnitedPayment.Hubs;
using UnitedPayment.Migrations;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.Enums;
using UnitedPayment.Persistance.context;
using UnitedPayment.Repository;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(
        AppDbContext context,
        IHubContext<ChatHub> hubContext,
        IRepository<Employee> userRepo) : ControllerBase

    {
        /// <summary>
        /// Get Chat
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> getChats(int toUserId, CancellationToken cancellationToken)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            Log.Information("Getting chats attempt from email : {@email}", email);
            var user = (await userRepo.GetAll(x => x.Email == email)).First();
            List<Chat> chats = await context.Chats.Where(p => p.UserId == user.Id && p.toUserId == toUserId
            || p.UserId == p.toUserId && p.toUserId == p.UserId).OrderBy(p => p.Date).ToListAsync(cancellationToken);
            return Ok(chats);
        }

        /// <summary>
        /// Send message
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequestDTO request, CancellationToken cancellationToken)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = (await userRepo.GetAll(x => x.Email == email)).First();

            Employee receiverUser = await userRepo.FindByIdAsync(request.toUserId);
            if(user.Role==UserRole.Employee && receiverUser.Role == UserRole.Employee)
            {
                return Forbid("Bir isci diger isciye mesaj gondere bilmez");
            }

            Chat chat = new()
            {
                UserId = user.Id,
                toUserId = request.toUserId,
                Message = request.Message,
                Date = DateTime.UtcNow
            };
            await context.AddAsync(chat, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var userConnection = ChatHub.Employees.FirstOrDefault(p => p.Value == chat.toUserId);

            if (!userConnection.Equals(default(KeyValuePair<string, string>)) && !string.IsNullOrEmpty(userConnection.Key))
            {
                await hubContext.Clients.Client(userConnection.Key).SendAsync("Messages", chat);
            }

            return Ok(chat);

        }

    }
}
