using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using GChat.ViewModels;
using GChat.Models;
using GChat.Services;

#nullable disable

namespace GChat
{

    public class ChatHub : Hub
    {
        private readonly ILogger _logger;
        private ChatContext _db;
        private RoomService _roomService;

        public ChatHub(ILogger<ChatHub> logger, ChatContext context, RoomService roomService)
        {
            _logger = logger;
            _db = context;
            _roomService = roomService;
        }

        [Authorize]
        public async Task Send(string pathName, string text)
        {
            string userName = Context.User.Identity.Name;

            DateTime now = DateTime.Now;

            ItemMessage itemMessage = new ItemMessage
            {
                Text = text,
                PublishedDate = DateTime.Now,
                UserName = userName
            };

            long chatId = Convert.ToInt64(pathName.Split('/')[^1]);

            await Clients.Group(_roomService[Context.ConnectionId]).SendAsync("Receive", itemMessage, Context.ConnectionId);

            User currentUser = _db.Users.FirstOrDefault(user => user.Login == userName);

            await _db.Messages.AddAsync(new Message
            {
                UserForeignKey = currentUser.UserId,
                ChatForeignKey = chatId,
                Text = text,
                PublishedDate = new DateTimeOffset(now).ToUnixTimeMilliseconds(),
            });

            _db.SaveChanges();
        }

        public async Task JoinRoom(string chatId)
        {
            _roomService[Context.ConnectionId] = chatId;

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            await Clients.Group(chatId).SendAsync("SetStatus", "online", Context.User.Identity.Name);

            _logger.LogInformation($"{Context.ConnectionId} join room {chatId}.");
        }
        private async Task LeaveRoom()
        {
            string roomId = _roomService[Context.ConnectionId];

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            _roomService.Remove(Context.ConnectionId);

            _logger.LogInformation($"{Context.ConnectionId} leave room {roomId}");
        }

        public async override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Connection Id: {Context.ConnectionId} is online.");

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Connection Id: {Context.ConnectionId} is offline.");

            await Clients.Group(_roomService[Context.ConnectionId]).SendAsync("SetStatus", "offline", Context.User.Identity.Name);

            await LeaveRoom();

            await base.OnDisconnectedAsync(exception);
        }
    }
}