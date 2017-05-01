using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetroShareApi.Connection;
using RetroShareApi.Request;
using System.Collections.Concurrent;
using System.Threading;

namespace BotHost
{
	class Program
	{
		static BlockingCollection<ChatMessage> msgqueue = new BlockingCollection<ChatMessage>();

		static void Main(string[] args)
		{
			IConnection con = new HTTPConnection(new Uri(@"http://localhost:9090"));
			Dictionary<string, RetroShareApi.Request.Chat.Messages> requests = new Dictionary<string, RetroShareApi.Request.Chat.Messages>();

			Thread t = new Thread(Consume);
			t.Start();

			while(true) {
				RetroShareApi.Request.Chat.Lobbies lobbies = new RetroShareApi.Request.Chat.Lobbies(con);
				foreach(RetroShareApi.Response.Chat.Lobby lobby in lobbies.Execute().data) {
					if(!lobby.subscribed || lobby.is_broadcast)
						continue;
					RetroShareApi.Request.Chat.Messages msgreq;
					bool found = requests.TryGetValue(lobby.id, out msgreq);
					if(!found) {
						msgreq = new RetroShareApi.Request.Chat.Messages(con, lobby.chat_id);
						requests.Add(lobby.id, msgreq);
					}

					foreach(RetroShareApi.Response.Chat.Message msg in msgreq.Execute().data) {
						BotHost.ChatMessage chatmsg = new ChatMessage();
						chatmsg.nick = msg.author_name;
						chatmsg.chatid = lobby.chat_id;
						chatmsg.text = msg.msg;
						chatmsg.time = (new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).AddSeconds(msg.send_time).ToLocalTime();
						msgqueue.Add(chatmsg);
					}
				}
				Thread.Sleep(1000);
			}
		}

		private static void Consume()
		{
			while(true) {
				ChatMessage msg = msgqueue.Take();
				Console.Out.WriteLine(msg.ToString());
			}
		}
	}

	public class ChatMessage
	{
		public string nick;
		public string chatid;
		public DateTime time;
		public string text;

		public override string ToString()
		{
			return time.ToString() + " " + nick + ": " + text;
		}
	}
}
