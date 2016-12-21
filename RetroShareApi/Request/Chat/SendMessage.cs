using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Request.Chat
{
    public class SendMessage : Request<object>
    {
        public SendMessage(string chatid) : base("chat", "send_message")
        {
            this.chatid = chatid;
        }

        public string chatid
        {
            get;
            private set;
        }

        private string _message;

        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                data = "{\"chat_id\":\"" + chatid + "\",\"msg\":\"" + _message + "\"}";
            }
        }
    }
}
