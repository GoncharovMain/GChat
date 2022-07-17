using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GChat.ViewModels;
using GChat.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using GChat;

#nullable disable

namespace Controllers
{
    public class ChatController : Controller
    {
        private ChatContext _db;
        private IHubContext<ChatHub> _hubContext;

        [ViewData]
        public long CurrentChatId { get; set; }

        public ChatController(ChatContext context, IHubContext<ChatHub> hubContext)
        {
            _db = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Chat(long? id)
        {
            Chat chat = _db.Chats.FirstOrDefault(chat => chat.ChatId == id);

            string name = this.User.Identity.Name;

            User currentUser = _db.Users.FirstOrDefault(user => user.Login == name);

            if (chat != null)
            {
                CurrentChatId = id ?? 1;

                return View();
            }

            return Content($"Чата с id: {id} не существует");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ICollection<User> users = _db.Users.ToList();

            ViewBag.Users = new SelectList(users, "UserId", "Login");

            var chatModel = new ChatModel
            {
                SelectedUsers = users.Select(user => user.Login).ToList(),
                IdMembers = new SelectList(users, "UserId", "Login").ToList()
            };

            return View(chatModel);
        }

        [HttpPost]
        public IActionResult Create(ChatModel model)
        {
            string name = this.User.Identity.Name;

            if (ModelState.IsValid)
            {
                User currentUser = _db.Users.FirstOrDefault(user => user.Login == name);

                long[] ids = model.SelectedUsers.Select(id => Convert.ToInt64(id)).ToArray();

                var chats = _db.Chats
                        .Include(chat => chat.Members);

                List<User> users = _db.Users
                    .Include(user => user.Chats)
                    .Where(user => ids.Contains(user.UserId))
                    .AsQueryable().ToList();

                Chat chat = new Chat
                {
                    Name = model.ChatName,
                    Members = users
                };

                foreach (User user in users)
                {
                    user.Chats.Add(chat);
                }

                if (ids.Contains(currentUser.UserId) == false)
                {
                    chat.Members.Add(currentUser);
                    currentUser.Chats.Add(chat);
                }

                _db.Chats.Add(chat);
                _db.SaveChanges();
            }

            return RedirectToAction("Create", "Chat");
        }

        [HttpGet]
        public IActionResult MyChats()
        {
            string name = this.User.Identity.Name;

            User currentUser = _db.Users
                .Include(user => user.Chats)
                .FirstOrDefault(user => user.Login == name);

            return View(currentUser.Chats);
        }

    }
}