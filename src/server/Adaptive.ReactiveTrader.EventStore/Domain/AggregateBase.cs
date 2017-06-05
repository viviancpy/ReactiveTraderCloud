using System.Collections.Generic;
using Adaptive.ReactiveTrader.EventStore.Process;

namespace Adaptive.ReactiveTrader.EventStore.Domain
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly List<WriteEvent> _pendingEvents = new List<WriteEvent>();

        public abstract string StreamPrefix { get; }
        public abstract string Identifier { get; }
        public int Version { get; private set; } = -1;

        void IAggregate.ApplyEvent(object @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;
        }

        public ICollection<WriteEvent> GetUncommittedEvents()
        {
            return _pendingEvents;
        }

        public void ClearUncommittedEvents()
        {
            _pendingEvents.Clear();
        }

        protected void RaiseEvent(object @event)
        {
            ((IAggregate)this).ApplyEvent(@event);
            _pendingEvents.Add(new WriteEvent(@event));
        }
    }
}