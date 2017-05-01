using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RetroShareApi.Response
{
	public class Response<DataType>
	{
		public Response() { }

		public string returncode;
		public string debug_msg;
		public DataType data;
		public int? statetoken = null;

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}

//{"data":[],"debug_msg":"","returncode":"ok"}