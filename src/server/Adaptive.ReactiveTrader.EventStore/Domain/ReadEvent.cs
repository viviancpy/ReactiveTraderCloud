using System.Collections.Generic;

namespace Adaptive.ReactiveTrader.EventStore.Domain
{
    public static class ReadEvent
    {
        public static ReadEvent<T> Create<T>(string stream,
                                             string eventType,
                                             int number,
                                             T payload,
                                             IReadOnlyDictionary<string, string> metadata = null)
        {
            return new ReadEvent<T>(stream, eventType, number, payload, metadata);
        }
    }

    public class ReadEvent<T> : IReadEvent<T>
    {
        public ReadEvent(string streamId, string eventType, int eventNumber, T payload, IReadOnlyDictionary<string, string> metadata = null)
        {
            StreamId = streamId;
            EventType = eventType;
            EventNumber = eventNumber;
            Payload = payload;
            Metadata = metadata ?? new Dictionary<string, string>();
        }

        public string StreamId { get; }
        public string EventType { get; }
        public int EventNumber { get; }
        public T Payload { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}