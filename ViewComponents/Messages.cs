using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using GChat.Models;
using System.Linq;


#nullable disable

namespace GChat.ViewComponents
{
    public class ItemMessage
    {
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTime PublishedDate { get; set; }
    }

    public class Messages : ViewComponent
    {
        private ChatContext _db;
        public Messages(ChatContext context) => _db = context;

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            Chat chat = _db.Chats.FirstOrDefault(chat => chat.ChatId == id);

            List<ItemMessage> messages = await (
                from message in _db.Messages
                from user in _db.Users
                where message.UserForeignKey == user.UserId
                where message.ChatForeignKey == id
                orderby message.PublishedDate
                select new ItemMessage
                {
                    UserName = user.Login,
                    Text = message.Text,
                    PublishedDate = DateTimeOffset.FromUnixTimeMilliseconds(message.PublishedDate).LocalDateTime
                }).ToListAsync();

            return View(new Tuple<string, List<ItemMessage>>(chat.Name, messages));
        }
    }
}