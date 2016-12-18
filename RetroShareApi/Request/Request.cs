using RetroShareApi.Connection;
using RetroShareApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RetroShareApi.Request
{
    public class Request <RespType>
    {
        public Request(string sector, string function = "", string data = null)
        {
            this.sector = sector;
            this.function = function;
            this.data = data;
        }

        public virtual string sector
        {
            get;
            protected set;
        }

        public virtual string function
        {
            get;
            protected set;
        }

        public virtual string data
        {
            get;
            protected set;
        }

        public virtual Response<RespType> Execute(IConnection connection)
        {
            return JsonConvert.DeserializeObject<Response<RespType>>(connection.SendRequest(sector, function, data));
        }
    }
}
