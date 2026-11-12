using System;
using UnityEngine;

namespace IndianBikeLife.Progression.Economy
{
    /// <summary>
    /// Encapsulates player virtual currency balances and enforces non-negative constraints.
    /// Uses integer storage to avoid floating-point precision errors.
    /// </summary>
    [Serializable]
    public class PlayerWallet
    {
        [SerializeField] private int _silverCoins = 1000;
        [SerializeField] private int _goldCoins = 10;

        public int SilverCoins => _silverCoins;
        public int GoldCoins => _goldCoins;

        public PlayerWallet() { }

        public PlayerWallet(int initialSilver, int initialGold)
        {
            _silverCoins = Mathf.Max(0, initialSilver);
            _goldCoins = Mathf.Max(0, initialGold);
        }

        public int GetBalance(CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.SilverCoins => _silverCoins,
                CurrencyType.GoldCoins => _goldCoins,
                _ => 0
            };
        }

        public bool CanAfford(CurrencyType currency, int amount)
        {
            if (amount < 0) return false;
            return GetBalance(currency) >= amount;
        }

        public bool Credit(CurrencyType currency, int amount, out int newBalance)
        {
            if (amount < 0)
            {
                newBalance = GetBalance(currency);
                return false;
            }

            switch (currency)
            {
                case CurrencyType.SilverCoins:
                    // Prevent integer overflow
                    if ((long)_silverCoins + amount > int.MaxValue)
                        _silverCoins = int.MaxValue;
                    else
                        _silverCoins += amount;
                    newBalance = _silverCoins;
                    return true;

                case CurrencyType.GoldCoins:
                    if ((long)_goldCoins + amount > int.MaxValue)
                        _goldCoins = int.MaxValue;
                    else
                        _goldCoins += amount;
                    newBalance = _goldCoins;
                    return true;

                default:
                    newBalance = 0;
                    return false;
            }
        }

        public bool Debit(CurrencyType currency, int amount, out int newBalance)
        {
            if (amount < 0 || !CanAfford(currency, amount))
            {
                newBalance = GetBalance(currency);
                return false;
            }

            switch (currency)
            {
                case CurrencyType.SilverCoins:
                    _silverCoins -= amount;
                    newBalance = _silverCoins;
                    return true;

                case CurrencyType.GoldCoins:
                    _goldCoins -= amount;
                    newBalance = _goldCoins;
                    return true;

                default:
                    newBalance = 0;
                    return false;
            }
        }

        public void SetBalances(int silver, int gold)
        {
            _silverCoins = Mathf.Max(0, silver);
            _goldCoins = Mathf.Max(0, gold);
        }
    }
}

