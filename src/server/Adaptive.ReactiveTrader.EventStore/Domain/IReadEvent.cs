using System.Collections.Generic;

namespace Adaptive.ReactiveTrader.EventStore.Domain
{
    public interface IReadEvent<out T>
    {
        string StreamId { get; }
        string EventType { get; }
        int EventNumber { get; }
        T Payload { get; }
        IReadOnlyDictionary<string, string> Metadata { get; }
    }
}