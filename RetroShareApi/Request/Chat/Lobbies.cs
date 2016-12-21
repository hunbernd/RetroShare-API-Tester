using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Request.Chat
{
    public class Lobbies : Request<List<Response.Chat.Lobby>>
    {
        public Lobbies() : base("chat", "lobbies") { }
    }
}
