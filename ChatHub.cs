using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using GChat.ViewModels;
using GChat.Models;

#nullable disable

namespace GChat
{
    public class ChatHub : Hub
    {
        private ChatContext _db;
        public ChatHub(ChatContext context)
        {
            _db = context;
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

            await Clients.All.SendAsync("Receive", itemMessage);

            User currentUser = _db.Users.FirstOrDefault(user => user.Login == userName);

            await _db.Messages.AddAsync(new Message
            {
                UserForeignKey = currentUser.UserId,
                ChatForeignKey = Convert.ToInt64(pathName.Split('/')[^1]),
                Text = text,
                PublishedDate = new DateTimeOffset(now).ToUnixTimeMilliseconds(),
            });

            _db.SaveChanges();
        }

        public async override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connection Id: {Context.ConnectionId} online.");

            await Clients.Others.SendAsync("SetStatus", "online", Context.User.Identity.Name);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Connection Id: {Context.ConnectionId} offline.");

            await Clients.Others.SendAsync("SetStatus", "offline", Context.User.Identity.Name);

            await base.OnDisconnectedAsync(exception);
        }
    }
}