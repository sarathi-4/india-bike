using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Progression.Economy;

namespace IndianBikeLife.Core.Events.GameEvents
{
    public readonly struct CurrencyEarnedEvent : IGameEvent
    {
        public CurrencyType Currency { get; }
        public int Amount { get; }
        public int NewBalance { get; }
        public string Source { get; }

        public CurrencyEarnedEvent(CurrencyType currency, int amount, int newBalance, string source)
        {
            Currency = currency;
            Amount = amount;
            NewBalance = newBalance;
            Source = source;
        }
    }

    public readonly struct CurrencySpentEvent : IGameEvent
    {
        public CurrencyType Currency { get; }
        public int Amount { get; }
        public int NewBalance { get; }
        public string Reason { get; }

        public CurrencySpentEvent(CurrencyType currency, int amount, int newBalance, string reason)
        {
            Currency = currency;
            Amount = amount;
            NewBalance = newBalance;
            Reason = reason;
        }
    }

    public readonly struct TransactionFailedEvent : IGameEvent
    {
        public CurrencyType Currency { get; }
        public int RequestedAmount { get; }
        public int CurrentBalance { get; }
        public string Reason { get; }

        public TransactionFailedEvent(CurrencyType currency, int requestedAmount, int currentBalance, string reason)
        {
            Currency = currency;
            RequestedAmount = requestedAmount;
            CurrentBalance = currentBalance;
            Reason = reason;
        }
    }
}
