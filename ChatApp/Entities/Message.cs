using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public virtual UserInfo Sender { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
