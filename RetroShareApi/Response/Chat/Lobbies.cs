using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Response.Chat
{
	public class Lobby
	{
		public bool auto_subscribe;
		public string chat_id;
		public string gxs_id;
		public string id;
		public bool is_broadcast;
		public bool is_private;
		public string name;
		public bool subscribed;
		public string topic;
		public int unread_msg_count;
	}
}

//"auto_subscribe":false,"chat_id":"L0F1EE2C4D0982D13","gxs_id":"2d7fdbc8d6894c131a7588fd05c816fb","id":"1089557494811340051","is_broadcast":false,"is_private":false,"name":"General Chats","subscribed":true,"topic":"","unread_msg_count":"1"