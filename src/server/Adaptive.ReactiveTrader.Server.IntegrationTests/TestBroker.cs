using System;
using System.IO;
using System.Threading.Tasks;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Fluent;
using WampSharp.WebsocketsPcl;
using Adaptive.ReactiveTrader.Messaging.WebSocket;
using Newtonsoft.Json;
using Websockets;

namespace Adaptive.ReactiveTrader.Server.IntegrationTests
{
    public class TestBroker
    {
        private readonly IWampChannel _channel;

        public TestBroker()
        {
            var configFile = Path.Combine(Directory.GetCurrentDirectory(), "integration-test.config.json");
            var brokerEndpoint = File.Exists(configFile) ? JsonConvert.DeserializeObject<IntegrationTestConfig>(File.ReadAllText(configFile)).Endpoint : TestAddress.Broker;

            Console.WriteLine(brokerEndpoint);

            WebSocketFactory.Init(() => new ClientWebSocketConnection());

            _channel = new WampChannelFactory()
                .ConnectToRealm("com.weareadaptive.reactivetrader")
                .WebSocketTransport(brokerEndpoint)
                .JsonSerialization()
                .Build();

            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        }

        public async Task<IWampChannel> OpenChannel()
        {
            await _channel.Open();
            return _channel;
        }
    }

    internal sealed class IntegrationTestConfig
    {
        public string Endpoint { get; set; }
    }
}