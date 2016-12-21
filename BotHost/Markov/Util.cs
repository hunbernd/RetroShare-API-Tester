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
            Console.Out.WriteLine(string.Format(format, args));
        }

        internal static void SendMessage(string chatid, string v)
        {
            Console.Out.WriteLine(v);
        }
    }
}
