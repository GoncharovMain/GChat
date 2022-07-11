using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GChat.Models;
using GChat.ViewModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


#nullable disable

namespace GChat.Controllers
{
    public class AccountController : Controller
    {
        private ChatContext _db;

        public AccountController(ChatContext context)
        {
            _db = context;
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

            _db.SaveChanges();

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

            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(user => user.Login == model.Login && user.Password == model.Password);

                if (user == null)
                {
                    long lastId = _db.Users.OrderBy(user => user.UserId).Last().UserId;

                    _db.Users.Add(new User
                    {
                        UserId = lastId + 1,
                        Login = model.Login,
                        Password = model.Password,
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        Birthday = new DateTimeOffset(model.Birthday).ToUnixTimeSeconds(),
                        Sex = model.Sex,
                    });

                    await _db.SaveChangesAsync();
                    Console.WriteLine($"SAVE MODEL: {model.Login} {model.Password} {model.Sex}");

                    await Authenticate(model.Login);
                }
                else
                {
                    ModelState.AddModelError("", "Не верный логин и (или) пароль");
                }
            }

            return View(model);
        }

        // public IActionResult Index()
        // {
        //     List<User> result = _db?.Users?.ToList();

        //     _db?.Add(new User
        //     {
        //         Login = "Goncharov",
        //         Password = "qwerty",
        //         FirstName = "Юрий",
        //         SecondName = "Гончаров",
        //         Birthday = new DateTimeOffset(new DateTime(1999, 1, 9)).ToUnixTimeSeconds(),
        //         Friends = null
        //     });

        //     _db?.SaveChanges();

        //     // long time = new DateTimeOffset(dateTime1).ToUnixTimeSeconds();
        //     // var dateTime2 = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).LocalDateTime;
        //     // var dateTime2 = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).UtcDateTime;


        //     return View(result);
        // }

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