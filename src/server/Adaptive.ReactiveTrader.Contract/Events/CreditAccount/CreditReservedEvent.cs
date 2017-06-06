namespace Adaptive.ReactiveTrader.Contract.Events.CreditAccount
{
    public class CreditReservedEvent
    {
        public CreditReservedEvent(string accountName, long tradeId)
        {
            AccountName = accountName;
            TradeId = tradeId;
        }

        public string AccountName { get; }
        public long TradeId { get; }
    }
}