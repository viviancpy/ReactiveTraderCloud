using System.Collections.Generic;
using Adaptive.ReactiveTrader.EventStore.Process;

namespace Adaptive.ReactiveTrader.EventStore.Domain
{
    public interface IAggregate
    {
        string StreamPrefix { get; }
        string Identifier { get; }
        int Version { get; }
        void ApplyEvent(object @event);
        ICollection<WriteEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}