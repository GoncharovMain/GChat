using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GChat.Models;
using GChat.ViewModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using AutoMapper;
using GChat.Mappers;

#nullable disable

namespace GChat.Controllers
{
    public class AccountController : Controller
    {
        private ChatContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ChatContext context, IMapper mapper, ILogger<AccountController> logger)
        {
            _db = context;
            _mapper = mapper;
            _logger = logger;
        }


        public IActionResult Profile(long? id)
        {
            User user = _db.Users.FirstOrDefault(user => user.UserId == id);

            return View(user);
        }

        public IActionResult Find()
        {
            List<User> users = _db.Users.ToList();
            return View(users);
        }

        public IActionResult Index()
        {

            User user1 = new User
            {
                UserId = 1,
                Login = "Goncharov",
                Password = "qwerty123",
                FirstName = "Юрий",
                SecondName = "Гончаров",
                Sex = "Male",
                Birthday = new DateTimeOffset(new DateTime(1999, 1, 9)).ToUnixTimeMilliseconds(),
            };

            User user2 = new User
            {
                UserId = 2,
                Login = "John",
                Password = "qwerty123",
                FirstName = "Джон",
                SecondName = "Ховстедер",
                Sex = "Male",
                Birthday = new DateTimeOffset(new DateTime(1985, 8, 4)).ToUnixTimeMilliseconds(),
            };

            Chat chat1 = new Chat
            {
                ChatId = 1,
                Name = "MainChat",
                Members = new List<User>() { user1, user2 }
            };


            _db.Messages.Add(new Message
            {
                MessageId = 1,
                UserForeignKey = 2,
                ChatForeignKey = 1,
                Text = "Hi",
                PublishedDate = 4121336,
            });

            _db.Messages.Add(new Message
            {
                MessageId = 2,
                UserForeignKey = 1,
                ChatForeignKey = 1,
                Text = "Hello",
                PublishedDate = 4121334,
            });

            user1.Chats = new List<Chat>() { chat1 };
            user2.Chats = new List<Chat>() { chat1 };

            _db.Users.Add(user1);
            _db.Users.Add(user2);

            _db.Chats.Add(chat1);

            //_db.SaveChanges();

            List<Chat> chats = _db.Chats.ToList();

            foreach (User user in chats[0].Members)
            {
                Console.WriteLine($"MEMBER: Login: {user.Login} firstname: {user.FirstName}");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(user => user.Login == model.Login && user.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(model.Login);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин и (или) пароль");
            }

            return RedirectToAction("MyChats", "Chat");
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(user => user.Login == model.Login);

                if (user == null)
                {
                    User newUser = _mapper.Map<User>(model);

                    _db.Users.Add(newUser);

                    await _db.SaveChangesAsync();

                    await Authenticate(model.Login);
                }
                else
                {
                    ModelState.AddModelError("", "Аккаунт с таким логином уже существует.");
                }
            }

            return View(model);
        }

        public async Task Authenticate(string login)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login)
            };

            ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
                );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(id)
                );
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}