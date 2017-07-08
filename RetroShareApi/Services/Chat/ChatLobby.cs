using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Services.Chat
{
	public class ChatLobby : IChat
	{
		//TODO Update with tokens and events
		//TODO Make them dynamic
		public bool autoSubscribe;
		public string chatId;
		public string gxsId;	//TODO replace with class
		public string id;
		public bool isBroadcast;
		public bool isPrivate;
		public string name;
		public bool subscribed;
		public string topic;
		public int unreadMsgCount;

		public readonly ChatService chatService;
		
		internal ChatLobby(ChatService service, Response.Chat.Lobby data)
		{
			this.chatService = service;

			this.autoSubscribe = data.auto_subscribe;
			this.chatId = data.chat_id;
			this.gxsId = data.gxs_id;
			this.id = data.id;
			this.isBroadcast = data.is_broadcast;
			this.isPrivate = data.is_private;
			this.name = data.name;
			this.subscribed = data.subscribed;
			this.topic = data.topic;
			this.unreadMsgCount = data.unread_msg_count;
		}

		public void SendMessage(string message)
		{
			Request.Chat.SendMessage req = new Request.Chat.SendMessage(chatService.Connection, chatId);
			req.message = message;
			req.ExecuteWithError();
		}

		public override bool Equals(object obj)
		{
			ChatLobby other = obj as ChatLobby;
			if(other == null)
				return false;
			else
				return this.chatId == other.chatId && this.chatService == other.chatService;
		}

		public override int GetHashCode()
		{
			return this.chatId.GetHashCode() ^ this.chatService.GetHashCode();
		}
	}
}
