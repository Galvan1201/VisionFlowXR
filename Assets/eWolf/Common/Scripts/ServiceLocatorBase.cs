using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace eWolf.Common
{
    public abstract class ServiceLocatorBase
    {
        protected static ServiceLocatorBase _instance = null;

        protected IDictionary<Type, object> _services = new Dictionary<Type, object>();

        public T GetService<T>()
        {
            try
            {
                return (T)_services[typeof(T)];
            }
            catch
            {
                IEnumerable<T> services = MonoBehaviour.FindObjectsOfType<MonoBehaviour>().OfType<T>();
                if (services.Any())
                {
                    InjectService<T>(services.First());
                    return (T)_services[typeof(T)];
                }
            }
            return default(T);
        }

        public void InjectService<T>(object service)
        {
            Type t2 = typeof(T);
            _services[t2] = service;
        }
    }
}