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

        private Dictionary<Type, Service> servicecache;

        public ServiceManager(IConnection connection)
        {
            this.connection = connection;
            servicecache = new Dictionary<Type, Service>();
        }

        public T getService<T>() where T : Service, new()
        {
            if (servicecache.ContainsKey(typeof(T)))
            {
                return (T)servicecache[typeof(T)];
            }
            else
            {
                T service = new T();
                servicecache.Add(typeof(T), service);
                return service;
            }
        }
    }
}
