namespace BrokerAdapter.BackTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class BackTestAdapter : ITradingAdapter
    {
        #region Constants

        private const decimal DolarsByPip = 0.0001m;

        private const string OrderSideBuy = "buy";

        #endregion

        #region Fields

        private readonly AccountInformation accountInformation = new AccountInformation();

        private readonly List<Trade> trades = new List<Trade>();

        private decimal balancePips;

        #endregion

        #region Public Methods and Operators

        public void CloseTrade(int accountId, long tradeId)
        {
            //TODO : CALCULATE NEW BALANCE
            this.trades.RemoveAll(x => x.Id == tradeId && x.AccountId == accountId);
        }

        public AccountInformation GetAccountInformation(int accountId)
        {
            //TODO: Return a copy
            return this.accountInformation;
        }

        public Trade GetOpenTrade(int accountId, string instrument = null)
        {
            return this.trades.FirstOrDefault(x => x.AccountId == accountId && (instrument == null || x.Instrument == instrument));
        }

        public bool HasOpenOrder(int accountId, string instrument = null)
        {
            //EXECUTION IS IMMEDIATE
            return false;
        }

        public bool HasOpenTrade(int accountId, string instrument = null)
        {
            return this.trades.Any(x => x.AccountId == accountId && (instrument == null || x.Instrument == instrument));
        }

        public void PlaceOrder(Order order)
        {
            this.trades.Add(new Trade
            {
                AccountId = order.AcountId,
                Instrument = order.Instrument,
                Id = this.trades.Count + 1,
                Price = order.Price, //Slippage model
                Side = order.Side,
                StopLoss = order.StopLoss,
                TakeProfit = order.TakeProfit,
                Time = order.Timestamp,
                TrailingStop = order.TrailingStop,
                Units = order.Units
            });
        }

        public void SetRate(Rate newRate)
        {
            //TODO: Calculate pip fraction to support YEN
            //TODO: Update balance
            //TODO: Check margin call
            //TODO: Handle error on leverage and insuficient funds

            var currentTrade = this.trades.FirstOrDefault();
            if (currentTrade == null)
            {
                return;
            }

            decimal amountToCompare;
            if (currentTrade.Side == OrderSideBuy)
            {
                if (currentTrade.TrailingStop > 0)
                {
                    var newTrailingAmount = newRate.Bid - currentTrade.TrailingStop * 0.0001m;

                    currentTrade.TrailingAmount = newTrailingAmount >= currentTrade.TrailingAmount
                        ? newTrailingAmount
                        : currentTrade.TrailingAmount;
                    amountToCompare = currentTrade.TrailingAmount;
                }
                else
                {
                    currentTrade.TrailingAmount = 0;
                    amountToCompare = currentTrade.StopLoss;
                }
            }
            else
            {
                if (currentTrade.TrailingStop > 0)
                {
                    var newTrailingAmount = newRate.Ask + currentTrade.TrailingStop * 0.0001m;
                    currentTrade.TrailingAmount = newTrailingAmount < currentTrade.TrailingAmount
                        ? newTrailingAmount
                        : currentTrade.TrailingAmount;
                    amountToCompare = currentTrade.TrailingAmount;
                }
                else
                {
                    currentTrade.TrailingAmount = 0;
                    amountToCompare = currentTrade.StopLoss;
                }
            }

            var gainLoss = 0m;
            if (currentTrade.Side == OrderSideBuy)
            {
                if (newRate.Bid >= amountToCompare) return;

                gainLoss = (currentTrade.Price - amountToCompare) / DolarsByPip;
                Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                this.balancePips += gainLoss * currentTrade.Units * DolarsByPip;
                Console.WriteLine("{1} - Balance = {0}", this.balancePips, currentTrade.Time);
                this.trades.Remove(currentTrade);
            }
            else
            {
                if (newRate.Ask <= amountToCompare) return;

                gainLoss = (amountToCompare - currentTrade.Price) / DolarsByPip;
                Console.WriteLine("Stop loss triggered=>Gain/Loss={0}", gainLoss);
                this.balancePips += gainLoss * currentTrade.Units * DolarsByPip;
                Console.WriteLine("{1} - Balance = {0}", this.balancePips, currentTrade.Time);
                this.trades.Remove(currentTrade);
            }
        }

        public void UpdateTrade(Trade updatedTrade)
        {
            var trade = this.trades.FirstOrDefault(x => x.Id == updatedTrade.Id);
            if (trade == null)
            {
                throw new Exception($"No such trade with Id = {updatedTrade.Id}");
            }

            trade.StopLoss = updatedTrade.StopLoss;
            trade.TakeProfit = updatedTrade.TakeProfit;
            trade.TrailingAmount = updatedTrade.TrailingAmount;
        }

        #endregion

        //No slipage, no spread, instant execution
    }
}