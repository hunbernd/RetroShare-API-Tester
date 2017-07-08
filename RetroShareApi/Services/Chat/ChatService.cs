using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RetroShareApi.Services.Chat
{
	public class ChatService : Service
	{
		private Dictionary<string, RetroShareApi.Request.Chat.Messages> requests = new Dictionary<string, RetroShareApi.Request.Chat.Messages>();
		public delegate void NewChatMessageHandler(IChat source, string author_id, string author_name, DateTime send_time, string message);
		public event NewChatMessageHandler OnChatMessage;
		Timer timer;

		public ChatService(ServiceManager servicemanager) : base(servicemanager)
		{
			timer = new Timer(CheckNewMessages, null, 0, 1000);
		}

		private void CheckNewMessages(Object stateInfo)
		{
			if(OnChatMessage == null) return;
			foreach(ChatLobby lobby in SubscribedLobbies()) {
				Request.Chat.Messages msgreq;
				bool found = requests.TryGetValue(lobby.id, out msgreq);
				if(!found) {
					msgreq = new Request.Chat.Messages(this.Connection, lobby.chatId);
					requests.Add(lobby.id, msgreq);
				}				
				foreach(Response.Chat.Message msg in msgreq.Execute().data) {
					DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).AddSeconds(msg.send_time).ToLocalTime();
					OnChatMessage(lobby, msg.author_id, msg.author_name, time, msg.msg);
				}
			}

		}

		public IEnumerable<ChatLobby> Lobbies()
		{
			List<ChatLobby> list = new List<ChatLobby>();
			Request.Chat.Lobbies lobbies = new RetroShareApi.Request.Chat.Lobbies(this.Connection);
			foreach(RetroShareApi.Response.Chat.Lobby lobby in lobbies.ExecuteWithError()) {
				list.Add(new ChatLobby(this, lobby));
			}
			return list;
		}

		public IEnumerable<ChatLobby> SubscribedLobbies()
		{
			return Lobbies().Where(lobby => (lobby.subscribed && !lobby.isBroadcast));
		}
	}
}
