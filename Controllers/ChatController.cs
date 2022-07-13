using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Authorization;
using GChat.ViewModels;
using GChat.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


#nullable disable

namespace Controllers
{
    public class ChatController : Controller
    {
        private ChatContext _db;

        [ViewData]
        public long CurrentChatId { get; set; }

        public ChatController(ChatContext context)
        {
            _db = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(long chatId, MessageSenderModel model)
        {
            string name = this.User.Identity.Name;

            User currentUser = _db.Users.FirstOrDefault(user => user.Login == name);

            if (ModelState.IsValid)
            {
                await _db.Messages.AddAsync(new Message
                {
                    UserForeignKey = currentUser.UserId,
                    ChatForeignKey = chatId,
                    Text = model.Text,
                    PublishedDate = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(),
                });

                _db.SaveChanges();
            }

            return RedirectToAction("Chat", "Chat", new { id = chatId });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ICollection<User> users = _db.Users.ToList();

            ViewBag.Users = new SelectList(users, "UserId", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult Create(ChatModel model)
        {
            string name = this.User.Identity.Name;

            if (ModelState.IsValid)
            {
                User currentUser = _db.Users.FirstOrDefault(user => user.Login == name);
                long[] ids = model.IdMembers;

                var chats = _db.Chats
                        .Include(chat => chat.Members);

                List<User> users = _db.Users
                    .Include(user => user.Chats)
                    .Where(user => ids.Contains(user.UserId))
                    .AsQueryable().ToList();

                Chat chat = new Chat
                {
                    Name = model.Name,
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