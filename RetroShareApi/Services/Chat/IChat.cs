using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Services.Chat
{
	public interface IChat
	{
		void SendMessage(string message);
	}
}
