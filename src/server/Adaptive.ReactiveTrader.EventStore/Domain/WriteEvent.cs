using System.Collections.Generic;

namespace Adaptive.ReactiveTrader.EventStore.Domain
{
    public class WriteEvent
    {
        public WriteEvent(object payload, IReadOnlyDictionary<string, string> metadata = null)
        {
            Payload = payload;
            Metadata = metadata ?? new Dictionary<string, string>();
        }

        public object Payload { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}