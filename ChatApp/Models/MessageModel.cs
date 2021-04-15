using ChatApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class MessageModel
    {
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }


        public static MessageModel FromEntity(Message msg)
        {
            return new MessageModel()
            {
                Sender = msg.Sender.Username,
                Text = msg.Text,
                Timestamp = msg.Timestamp
            };
        }
    }
}
