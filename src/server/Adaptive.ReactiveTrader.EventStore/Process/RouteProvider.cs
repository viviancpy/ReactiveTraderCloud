using System;
using System.Collections.Generic;

namespace Adaptive.ReactiveTrader.EventStore.Process
{
    // Consider bring this class and EventHandlerRouter together, as they do essentially the same thing.
    public class RouteProvider
    {
        private readonly Dictionary<Type, Action<object>> _routes = new Dictionary<Type, Action<object>>();

        public void RegisterRoute<T>(Action<T> action)
        {
            _routes.Add(typeof(T), x => action((T)x));
        }

        public void DispatchToRoute(object item)
        {
            Action<object> route;
            if (_routes.TryGetValue(item.GetType(), out route))
            {
                route(item);
            }
            
            // No route - TODO - add default route with logging?
        }
    }
}