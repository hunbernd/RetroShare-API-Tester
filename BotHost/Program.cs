using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetroShareApi.Connection;
using RetroShareApi.Request;
using System.Collections.Concurrent;
using System.Threading;
using RetroShareApi.Services;
using RetroShareApi.Services.Chat;

namespace BotHost
{
	class Program
	{
		static void Main(string[] args)
		{
			IConnection con = new HTTPConnection(new Uri(@"http://localhost:1024"));
			ServiceManager sm = new ServiceManager(con);
			ChatService cs = (ChatService)sm.GetService("chat");
			cs.OnChatMessage += OnChatMessage;
			Console.ReadKey();
		}

		private static void OnChatMessage(IChat source, string author_id, string author_name, DateTime send_time, string message)
		{
			Console.Out.WriteLine(send_time.ToString() + " " + author_name + ": " + message);
			if(message.StartsWith("/ping"))
				source.SendMessage("pong");
		}
	}
}
