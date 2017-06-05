using System.Collections.Generic;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.EventStore.Domain;

namespace Adaptive.ReactiveTrader.EventStore.Process
{
    public interface IProcess
    {
        string StreamPrefix { get; }
        string Identifier { get; }
        int Version { get; }
        void Transition(IReadEvent<object> @event);
        IReadOnlyList<WriteEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
        Task DispatchMessages();
        void ClearUndispatchedMessages();
    }
}