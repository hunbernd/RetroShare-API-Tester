using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetroShareApi.Connection;
using RetroShareApi.Response;
using RetroShareApi.Response.Chat;

namespace RetroShareApi.Request.Chat
{
    public class Messages : Request<List<Response.Chat.Message>>
    {
        public Messages(IConnection connection, string chatid) : base(connection, "chat", "messages/" + chatid) { }

        private string begin_after = "begin";

        public override Response<List<Message>> Execute()
        {
            this.data = "{\"begin_after\":\"" + begin_after + "\"}";
            Response<List<Message>> answer = base.Execute();
            if(answer.data.Count > 0)
            {
                begin_after = answer.data.Last().id;
            }
            return answer;
        }
    }
}
