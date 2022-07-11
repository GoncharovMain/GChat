using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ChatController(ChatContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Chat(long? id)
        {
            Chat chat = _db.Chats.FirstOrDefault(chat => chat.ChatId == id);

            if (chat != null)
            {
                return View();
            }

            return Content($"Чата с id: {id} не существует");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(MessageSenderModel model)
        {
            ClaimsPrincipal user = this.User;

            string name = user.Identity.Name;

            User currentUser = _db.Users.FirstOrDefault(user => user.Login == name);

            if (ModelState.IsValid)
            {
                _db.Messages.Add(new Message
                {
                    UserForeignKey = currentUser.UserId,
                    ChatForeignKey = 1,
                    Text = model.Text,
                    PublishedDate = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(),
                });

                _db.SaveChanges();
            }

            return RedirectToAction("Chat", "Chat", new { id = 1 });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ICollection<User> users = _db.Users.ToList();

            ViewBag.Users = new SelectList(users, "UserId", "Login");

            Console.WriteLine($"VIEW_BAG: {ViewBag.Users}");

            return View();
        }

        [HttpPost]
        public IActionResult Create(ChatModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Chats.Add(new Chat
                {
                    Name = model.Name,
                    Members = model.Members
                });

                _db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MyChats()
        {
            List<Chat> chats = _db.Chats.ToList();
            return View(chats);
        }

    }
}