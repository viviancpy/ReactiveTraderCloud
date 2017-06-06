namespace Adaptive.ReactiveTrader.Server.TradeExecution.Commands
{
    public class RejectTradeCommand
    {
        public RejectTradeCommand(long tradeId, string reason)
        {
            TradeId = tradeId;
            Reason = reason;
        }

        public long TradeId { get; }
        public string Reason { get; }
    }
}