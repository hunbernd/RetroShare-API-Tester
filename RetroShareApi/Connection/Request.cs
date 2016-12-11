using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Connection
{
    public class Request
    {
        public Request(string sector, string function = "", string data = null)
        {
            this.sector = sector;
            this.function = function;
            this.data = data;
        }

        public string sector
        {
            get;
            protected set;
        }

        public string function
        {
            get;
            protected set;
        }

        public string data
        {
            get;
            protected set;
        }
    }
}
