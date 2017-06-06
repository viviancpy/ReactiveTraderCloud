namespace Adaptive.ReactiveTrader.Contract.Events.CreditAccount
{
    public class CreditLimitBreachedEvent
    {
        public CreditLimitBreachedEvent(string accountName, long tradeId)
        {
            AccountName = accountName;
            TradeId = tradeId;
        }

        public string AccountName { get; }
        public long TradeId { get; }
    }
}