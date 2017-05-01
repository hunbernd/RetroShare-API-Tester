using RetroShareApi.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Services
{
	public class ServiceManager
	{
		public readonly IConnection connection;

		private Dictionary<string, Service> servicecache;

		public ServiceManager(IConnection connection)
		{
			this.connection = connection;
			servicecache = new Dictionary<string, Service>();
		}

		public Service getService(string servicename)
		{
			if(servicecache.ContainsKey(servicename)) {
				return servicecache[servicename];
			} else {
				Service service;
				switch(servicename) {
					default:
						throw new Exception("Service does not exists");
				}

				servicecache.Add(servicename, service);
				return service;
			}
		}
	}
}
