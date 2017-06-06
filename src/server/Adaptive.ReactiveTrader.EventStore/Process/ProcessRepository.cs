using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.EventStore.Domain;
using EventStore.ClientAPI;
using Serilog;
using Serilog.Events;

namespace Adaptive.ReactiveTrader.EventStore.Process
{
    public class ProcessRepository : RepositoryBase, IProcessRepository, IDisposable
    {
        public ProcessRepository(IEventStoreConnection eventStoreConnection, EventTypeResolver eventTypeResolver)
            : base(eventStoreConnection, eventTypeResolver)
        {
        }

        public void Dispose()
        {
            // Nothing to do
        }

        public async Task<TProcess> GetByIdAsync<TProcess>(object id, Func<TProcess> factory) where TProcess : IProcess
        {
            var process = factory();
            var streamName = $"{process.StreamPrefix}{id}";

            if (Log.IsEnabled(LogEventLevel.Information))
            {
                Log.Information("Loading process {streamName} from Event Store", streamName);
            }

            var result = await ReadEventsAsync(streamName, e => process.Transition(e));
            process.ClearUncommittedEvents();
            process.ClearUndispatchedMessages();

            switch (result)
            {
                case SliceReadStatus.StreamNotFound:
                    // TODO - new exception type for Processes
                    throw new AggregateNotFoundException(id, typeof(TProcess));
                case SliceReadStatus.StreamDeleted:
                    // TODO - new exception type for Processes
                    throw new AggregateDeletedException(id, typeof(TProcess));
            }

            return process;
        }

        public async Task<TProcess> GetByIdOrCreateAsync<TProcess>(object id, Func<TProcess> factory) where TProcess : IProcess
        {
            var process = factory();
            var streamName = $"{process.StreamPrefix}{id}";

            if (Log.IsEnabled(LogEventLevel.Information))
            {
                Log.Information("Loading process {streamName} from Event Store", streamName);
            }

            var result = await ReadEventsAsync(streamName, e => process.Transition(e));
            process.ClearUncommittedEvents();
            process.ClearUndispatchedMessages();

            if (result == SliceReadStatus.StreamDeleted)
            {
                // TODO - new exception type for Processes
                throw new AggregateDeletedException(id, typeof(TProcess));
            }

            return process;
        }

        public async Task<int> SaveAsync(IProcess process, params KeyValuePair<string, string>[] extraHeaders)
        {
            var streamName = process.Identifier;
            var events = process.GetUncommittedEvents();
            if (events.Count == 0)
            {
                Log.Information("Process {streamName} has no events to save", streamName);
                return -1;
            }

            var expectedVersion = process.Version - events.Count;
            var commitId = Guid.NewGuid().ToString();

            if (Log.IsEnabled(LogEventLevel.Information))
            {
                Log.Information("Saving process {streamName}", streamName);
            }

            await process.DispatchMessages();

            // Only save after we've successfully dispatched all messages.
            var result = await WriteEventsAsync(streamName, expectedVersion, events, extraHeaders, commitId);

            if (Log.IsEnabled(LogEventLevel.Information))
            {
                Log.Information("Process {streamName} messages dispatched", streamName);
            }

            process.ClearUncommittedEvents();
            process.ClearUndispatchedMessages();

            if (Log.IsEnabled(LogEventLevel.Information))
            {
                Log.Information("Process {streamName} uncommitted events cleaned up", streamName);
                Log.Information("Process {streamName} undispatched messages cleaned up", streamName);
            }

            return result.NextExpectedVersion;
        }
    }
}