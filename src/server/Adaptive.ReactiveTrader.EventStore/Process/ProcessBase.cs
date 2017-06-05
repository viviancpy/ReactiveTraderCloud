using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.EventStore.Domain;
using Serilog;

namespace Adaptive.ReactiveTrader.EventStore.Process
{
    public abstract class ProcessBase : IProcess
    {
        private readonly RouteProvider _routeProvider = new RouteProvider();
        private readonly List<WriteEvent> _uncommittedEvents = new List<WriteEvent>();
        private readonly List<Message> _undispatchedMessages = new List<Message>();
        private int _lastProcessedEventNumber = -1;

        public abstract string StreamPrefix { get; }
        public abstract string Identifier { get; }
        public int Version { get; private set; } = -1;

        public void Transition(IReadEvent<object> @event)
        {
            // Events passed to this method come from two sources. The "live" source which is a projection stream (e.g. "trade_execution_proj"),
            // or from the process's stream itself (e.g. "tradeExecution-1"), which is where we replay events from when loading a process. We need
            // to differentiate whether this event is from the process stream or from the projection stream. This is so we can determine if we've
            // already processed a given event (i.e. an event from the projection stream) by keeping track of the last processed event number. The 
            // event number is stored as part of the metadata for events written to the process stream. There may be a better way to do this using
            // the "LinkTo" event type, which is essentially an event which links to another event.
            var isFromProcessStream = IsFromProcessStream(@event.StreamId);

            var eventNumber = isFromProcessStream
                ? int.Parse(@event.Metadata[MetadataKeys.ProcessEventNumber])
                : @event.EventNumber;

            if (eventNumber <= _lastProcessedEventNumber)
            {
                Log.Information("Event number {number} of type {type} has already been processed. Ignoring.", eventNumber, @event.EventType);
                return;
            }

            _routeProvider.DispatchToRoute(@event.Payload);
            _lastProcessedEventNumber = eventNumber;
            Version++;

            // If we're from the process stream, we don't write out the event again.
            if (isFromProcessStream) return;

            var metadata = new Dictionary<string, string>
            {
                { MetadataKeys.ProcessStreamId, @event.StreamId },
                { MetadataKeys.ProcessEventNumber, eventNumber.ToString() }
            };

            var writeEvent = new WriteEvent(@event.Payload, metadata);
            _uncommittedEvents.Add(writeEvent);
        }

        public IReadOnlyList<WriteEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents.AsReadOnly();
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public Task DispatchMessages()
        {
            return Task.WhenAll(_undispatchedMessages.Select(m => m()));
        }

        public void ClearUndispatchedMessages()
        {
            _undispatchedMessages.Clear();
        }

        protected void RegisterRoute<TEvent>(Action<TEvent> onEvent)
        {
            _routeProvider.RegisterRoute(onEvent);
        }

        protected void AddMessageToDispatch(Message message)
        {
            _undispatchedMessages.Add(message);
        }

        private bool IsFromProcessStream(string streamId)
        {
            return streamId.StartsWith(StreamPrefix);
        }
    }
}