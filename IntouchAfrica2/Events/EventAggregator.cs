using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.Events
{
    public class EventAggregator
    {
        private static EventAggregator _instance;
        private static object _instanceLock = new object();

        private EventAggregator()
        {
            _registrations = new Dictionary<Type, List<object>>();
        }

        //Temporary Singleton

        public static EventAggregator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                            _instance = new EventAggregator();
                    }
                }

                return _instance;
            }
        }

        private Dictionary<Type, List<object>> _registrations;

        public void Publish<T>(T @event) where T : DomainEvent
        {
            var eventType = typeof(T);
            if (_registrations.ContainsKey(eventType))
                foreach (var handler in _registrations[eventType].Cast<Action<T>>())
                    handler(@event);
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var eventType = typeof(T);
            if (_registrations[eventType] == null)
                _registrations[eventType] = new List<object>();

            _registrations[eventType].Add(handler);
        }
    }
}