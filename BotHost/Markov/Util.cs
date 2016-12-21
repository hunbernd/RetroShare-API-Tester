using RetroShareApi.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotHost.Markov
{
    public class Util
    {
        public static IConnection con;

        public static void SendNotice(string chatid, string format,
            params object[] args)
        {
            SendMessage(chatid, string.Format(format, args));
        }

        internal static void SendMessage(string chatid, string v)
        {
            Console.Out.WriteLine(v);
            RetroShareApi.Request.Chat.SendMessage msg = new RetroShareApi.Request.Chat.SendMessage(chatid);
            msg.message = v;
            msg.Execute(con);
        }
    }
}
