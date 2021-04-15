using ChatApp.Entities;
using ChatApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        public ChatContext context = new ChatContext();
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            LoginModel empty = new LoginModel();
            return View(empty);
        }
        [HttpPost]
        public async Task<IActionResult> Register(string login, string pswd)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Имя пользователя не указано!"
                };
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(pswd))
            {
                LoginModel model = new LoginModel()
                {
                    Login = login,
                    ErrorMessage = "Пароль не корректный!"
                };
                return View(model);
            }
            UserInfo usr = context.Users.FirstOrDefault(u => u.Username == login);
            if (usr != null)
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Данное имя пользователя уже используется"
                };
                return View(model);
            }
            else
            {
                UserInfo user = new UserInfo()
                {
                    Username = login,
                    PasswordHash = Crypto.HashPassword(pswd)
                };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Redirect("/Home/Index");
            }
        }
        [Authorize]
        public IActionResult UserPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginModel empty = new LoginModel();
            return View(empty);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
                };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        [HttpPost]
        public async Task<IActionResult> Login(string login, string pswd)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Имя пользователя не указано!"
                };
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(pswd))
            {
                LoginModel model = new LoginModel()
                {
                    Login = login,
                    ErrorMessage = "Пароль не корректный!"
                };
                return View(model);
            }
            UserInfo usr = context.Users.FirstOrDefault(u => u.Username == login);
            if (usr == null)
            {
                LoginModel model = new LoginModel()
                {
                    ErrorMessage = "Пользователя не существует!"
                };
                return View(model);
            }
            else if (!Crypto.VerifyHashedPassword(usr.PasswordHash,pswd))
            {
                LoginModel model = new LoginModel()
                {
                    ErrorMessage = "Неверный пароль!"
                };
                return View(model);
            }
            else
            {
                await Authenticate(login);
                return Redirect("/Home/UserPage");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Index");
        }
        [Authorize]
        public async Task<IActionResult> Chat()
        {
            //IQueryable<Message> model = context.Messages;
            //foreach (Message msg in model)
            //    await context.Entry(msg).Reference(m => m.Sender).LoadAsync();
            return View(context.Messages.ToList());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Chat(string message)
        {
            UserInfo user = context.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            Message msg = new Message()
            {
                Text = message,
                Timestamp = DateTime.Now,
                Sender = user
            };
            await context.Messages.AddAsync(msg);
            await context.SaveChangesAsync();
            return View(context.Messages.ToList());
        }
    }
}
