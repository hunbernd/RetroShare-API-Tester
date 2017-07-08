using RetroShareApi.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroShareApi.Services
{
	public abstract class Service
	{
		public readonly ServiceManager serviceManager;

		protected Service(ServiceManager servicemanager)
		{
			this.serviceManager = servicemanager;
		}

		public IConnection Connection
		{
			get { return serviceManager.connection; }
		}
	}
}
