using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GChat.Models;

#nullable disable

namespace GChat.ViewComponents
{
    public class Members : ViewComponent
    {
        private ChatContext _db;
        public Members(ChatContext context) => _db = context;

        public async Task<IViewComponentResult> InvokeAsync(long? id)
            => await Task.Run(() =>
            {
                Chat chat = _db.Chats
                .Include(chat => chat.Members)
                .FirstOrDefault(chat => chat.ChatId == id);

                if (chat != null)
                {
                    return View(chat.Members);
                }

                return View(new List<User>());
            });
    }
}
