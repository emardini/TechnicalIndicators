namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;

    public interface ITradingAdapter
    {
        bool HasOpenOrder(int accountId, string instrument = null);

        bool HasOpenTrade(int accountId, string instrument = null);

        Trade GetOpenTrade(int accountId, string instrument = null);

        void CloseTrade(int accountId, long tradeId);

        void PlaceOrder(Order order);

        void UpdateTrade(Trade updatedTrade);

        AccountInformation GetAccountInformation(int accountId);
    }
}