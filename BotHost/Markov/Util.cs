using RetroShareApi.Connection;
using RetroShareApi.Services.Chat;
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

        public static void SendNotice(IChat chatid, string format,
            params object[] args)
        {
            chatid.SendMessage(string.Format(format, args));
        }

        internal static void SendMessage(IChat chatid, string v)
        {
            Console.Out.WriteLine(v);
			chatid.SendMessage(v);
        }
    }
}
