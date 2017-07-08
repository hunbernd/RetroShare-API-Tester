using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Response.Chat
{
	public class Message
	{
		public string author_id;
		public string author_name;
		public string id;
		public bool incoming;
		//public List<string> links;
		public string msg;
		public long recv_time;
		public long send_time;
		public bool was_send;
	}
}