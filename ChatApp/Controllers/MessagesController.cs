using ChatApp.Entities;
using ChatApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ChatContext context = new ChatContext();
        [HttpGet]
        public IEnumerable<MessageModel> Get(string username, string password)
        {
            UserInfo sender = context.Users.SingleOrDefault(usr => usr.Username == username);
            if (sender != null && Crypto.VerifyHashedPassword(sender.PasswordHash, password))
            {
                IQueryable<Message> allMesages = from msg in context.Messages
                                                 orderby msg.Timestamp
                                                 select msg;
                return allMesages.ToList().ConvertAll(msg => MessageModel.FromEntity(msg));
            }
            else
            {
                return null;
            }
        }
    }
}
