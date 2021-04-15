using ChatApp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        private ChatContext context = new ChatContext();

        [HttpGet]
        public async Task<string> GetAsync(string text, string username, string password)
        {
            //UserInfo sender = (from usr in context.Users
            //                  where usr.Username == username
            //                  select usr).SingleOrDefault();
            UserInfo sender = context.Users.SingleOrDefault(usr => usr.Username == username);
            if (sender!=null && Crypto.VerifyHashedPassword(sender.PasswordHash, password))
            {
                Message newMessage = new Message()
                {
                    Sender = sender,
                    Text = text,
                    Timestamp = DateTime.Now
                };
                await context.Messages.AddAsync(newMessage);
                await context.SaveChangesAsync();
                return "Сообщение отправлено";
            }
            else
            {
                return "Имя пользователя или пароль введены неверно";
            }
        }
    }
}
