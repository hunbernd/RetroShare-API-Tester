using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace RetroShareApi.Connection
{
    public class HTTPConnection : IConnection
    {
        const string URIADD = "api/v2";

        public HTTPConnection(Uri url)
        {
            this.url = url;
        }

        public Uri url
        {
            get;
            private set;
        }

        public string sendRequest(Request request)
        {
            string path = URIADD;
            if (!string.IsNullOrEmpty(request.sector)) path += "/" + request.sector;
            if (!string.IsNullOrEmpty(request.function)) path += "/" + request.function;
            Uri requri = new Uri(url, path);
            WebRequest webrequest = WebRequest.Create(requri);

            if (String.IsNullOrEmpty(request.data))
            {
                webrequest.Method = "GET";
            }
            else
            {
                webrequest.Method = "POST";
                byte[] tosend = Encoding.UTF8.GetBytes(request.data);
                webrequest.ContentLength = tosend.Length;
                webrequest.ContentType = "application/json";
                Stream dataStream = webrequest.GetRequestStream();
                dataStream.Write(tosend, 0, tosend.Length);
                dataStream.Close();
            }

            string responsestring = "";
            using (WebResponse response = webrequest.GetResponse()) { 
                Stream responsestream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responsestream);
                responsestring = reader.ReadToEnd();
                reader.Close();
                responsestream.Close();
            }
        
            return responsestring;
        }
    }
}
