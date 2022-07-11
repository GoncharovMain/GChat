using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GChat.Models;



#nullable disable

namespace GChat.ViewComponents
{
    public class Messages : ViewComponent
    {
        private ChatContext _db;
        public Messages(ChatContext context) => _db = context;

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            Chat chat = _db.Chats.FirstOrDefault(chat => chat.ChatId == id);

            List<Message> messages = new List<Message>();

            if (chat != null)
            {
                messages = await GetMessagesAsync(id);
            }

            Console.WriteLine($"MESSAGES COUNT: {messages.Count} ID:{id} {chat == null}");

            return View(messages);
        }

        private Task<List<Message>> GetMessagesAsync(long? id)
            => _db.Messages.Where(message => message.ChatForeignKey == id)
                .OrderBy(message => message.PublishedDate)
                .ToListAsync();

    }
}