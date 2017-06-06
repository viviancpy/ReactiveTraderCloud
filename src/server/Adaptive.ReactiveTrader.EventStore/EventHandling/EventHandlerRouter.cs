using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.EventStore.Domain;

namespace Adaptive.ReactiveTrader.EventStore.EventHandling
{
    public class EventHandlerRouter
    {
        private readonly Dictionary<Type, Func<IReadEvent<object>, Task>> _handlers = new Dictionary<Type, Func<IReadEvent<object>, Task>>();

        public void AddRoute<TEvent>(Func<IReadEvent<TEvent>, Task> handler)
        {
            _handlers.Add(typeof(TEvent), x => handler(ReadEvent.Create(x.StreamId, x.EventType, x.EventNumber, (TEvent)x.Payload)));
        }

        public bool CanRoute(object @event) => _handlers.ContainsKey(@event.GetType());

        public Task Route(IReadEvent<object> @event)
        {
            Func<IReadEvent<object>, Task> handler;
            return _handlers.TryGetValue(@event.Payload.GetType(), out handler) ? handler(@event) : Task.CompletedTask;
        }
    }
}