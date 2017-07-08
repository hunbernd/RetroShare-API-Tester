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
using IrcDotNet;

namespace BotHost
{
	class Program
	{
		private static DateTime starttime;
		public static readonly string prefix = "/";
		private static MarkovChainTextBot markov;

		static void Main(string[] args)
		{
			IConnection con = new HTTPConnection(new Uri(@"http://localhost:1024"));
			ServiceManager sm = new ServiceManager(con);
			ChatService cs = (ChatService)sm.GetService("chat");
			starttime = DateTime.Now;
			markov = new MarkovChainTextBot();
			cs.OnChatMessage += OnChatMessage;
			Console.ReadKey();
		}

		private static void OnChatMessage(IChat source, string author_id, string author_name, DateTime send_time, string message, bool incoming)
		{
			Console.Out.WriteLine(send_time.ToString() + " " + author_name + ": " + message);
			if(send_time > starttime) {
				if(message.StartsWith(prefix)) {
					char[] separator = new char[] { ' ' };
					string[] parameters = message.Split(separator, 10);
					string command = parameters.First().Substring(1);
					parameters = parameters.Skip(1).ToArray();

					if(command == "ping")
						source.SendMessage("pong");
					else if(command == "talk")
						markov.ProcessChatCommandTalk(source, command, parameters);
					else if(command == "stats")
						markov.ProcessChatCommandStats(source, command, parameters);
				}
			}

			if(!message.StartsWith(prefix) && incoming)
				markov.OnChannelMessageReceived(source, author_name, message);
		}
	}
}
