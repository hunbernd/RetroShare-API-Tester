using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Connection
{
	public interface IConnection
	{
		string SendRequest(string sector, string function = "", string data = null);
	}
}
