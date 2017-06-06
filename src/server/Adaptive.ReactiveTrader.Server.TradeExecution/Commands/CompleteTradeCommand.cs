namespace Adaptive.ReactiveTrader.Server.TradeExecution.Commands
{
    public class CompleteTradeCommand
    {
        public CompleteTradeCommand(long tradeId)
        {
            TradeId = tradeId;
        }

        public long TradeId { get; }
    }
}