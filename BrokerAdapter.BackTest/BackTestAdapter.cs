namespace BrokerAdapter.BackTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TechnicalIndicators;

    public class BackTestAdapter : ITradingAdapter
    {
        #region Constants

        private const string OrderSideBuy = "buy";

        private const decimal DebouncingLimit = 0.00001m;

        #endregion

        #region Fields

        private readonly AccountInformation accountInformation = new AccountInformation();

        private readonly List<Trade> trades = new List<Trade>();

        private Rate currentRate;

        private Rate currentAccountCurrencyRate;

        #endregion

        #region Public Methods and Operators

        public void CloseTrade(int accountId, long tradeId)
        {
            //TODO : CALCULATE NEW BALANCE
            var trade = this.trades.FirstOrDefault(x => x.Id == tradeId && x.AccountId == accountId);
            if(trade == null)
                return;

            if (this.currentRate == null || this.currentAccountCurrencyRate == null)
            {
                return;                
            }

            this.LiquidateTrade(this.currentRate, this.currentAccountCurrencyRate, trade, trade.Price);
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

        public void SetRate(Rate newRate, Rate accountCurrencyRate)
        {
            //TODO: Check margin call
            //TODO: Handle error on leverage and insuficient funds
            //TODO: Handle target profit
            this.currentRate = newRate;
            this.currentAccountCurrencyRate = accountCurrencyRate;

            var currentTrade = this.trades.FirstOrDefault();
            if (currentTrade == null)
            {
                return;
            }

            decimal amountToCompare;
            if (currentTrade.Side == OrderSideBuy) //In case of a long trade
            {
                if (currentTrade.TrailingStop > 0)
                {
                    var newTrailingAmount = newRate.Bid - currentTrade.TrailingStop * newRate.QuoteCurrency.GetPipFraction();

                    currentTrade.TrailingAmount = newTrailingAmount >= currentTrade.TrailingAmount
                        ? newTrailingAmount
                        : currentTrade.TrailingAmount;
                    amountToCompare = currentTrade.TrailingAmount;
                }
                else if (currentTrade.StopLoss > 0)
                {
                    currentTrade.TrailingAmount = 0;
                    amountToCompare = currentTrade.StopLoss;
                }
                else
                {
                    amountToCompare = 0; //TODO: To code a strategy without stops
                }
            }
            else  //In case of short trade
            {
                if (currentTrade.TrailingStop > 0)
                {
                    var newTrailingAmount = newRate.Ask + currentTrade.TrailingStop * newRate.QuoteCurrency.GetPipFraction();
                    currentTrade.TrailingAmount = newTrailingAmount < currentTrade.TrailingAmount
                        ? newTrailingAmount
                        : currentTrade.TrailingAmount;
                    amountToCompare = currentTrade.TrailingAmount;
                }
                else if (currentTrade.StopLoss > 0)
                {
                    currentTrade.TrailingAmount = 0;
                    amountToCompare = currentTrade.StopLoss;
                }
                else
                {
                    amountToCompare = 0;
                }
            }

            this.LiquidateTrade(newRate, accountCurrencyRate, currentTrade, amountToCompare);
        }

        private void LiquidateTrade(Rate newRate, Rate accountCurrencyRate, Trade currentTrade, decimal referencePrice)
        {
            var gainLoss = 0m;
            if (currentTrade.Side == OrderSideBuy)
            {
                if ((newRate.Bid - referencePrice) > DebouncingLimit) return;

                gainLoss = referencePrice - currentTrade.Price;
            }
            else
            {
                if (referencePrice - newRate.Ask > DebouncingLimit) return;

                gainLoss = currentTrade.Price - referencePrice;
            }

            Console.WriteLine("Stop loss triggered=>Gain/Loss={0} pips", gainLoss / newRate.QuoteCurrency.GetPipFraction());
            var gainLossInQuoteCurrency = gainLoss * currentTrade.Units;
            var gainLossConversionRate = this.GetAccountCurrencyRate(newRate, accountCurrencyRate);
            this.accountInformation.Balance += gainLossInQuoteCurrency * gainLossConversionRate;
            Console.WriteLine("{1} - Balance = {0}", this.accountInformation.Balance, currentTrade.Time);
            this.trades.Remove(currentTrade);
        }

        private decimal GetAccountCurrencyRate(Rate newRate, Rate accountCurrencyRate)
        {
            var quoteInstrument = newRate.QuoteCurrency.Safe().Trim().ToUpper();
            var baseInstrument = this.accountInformation.AccountCurrency.Safe().Trim().ToUpper();

            if (quoteInstrument == baseInstrument) return 1.00m;
            

            var price = (accountCurrencyRate.Ask + accountCurrencyRate.Bid)/2.00m;
            if (accountCurrencyRate.BaseCurrency == this.accountInformation.AccountCurrency)
            {
                return 1.00m / price;
            }

            return price;
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
            trade.TrailingStop = updatedTrade.TrailingStop;
        }

        #endregion

        //No slipage, no spread, instant execution
    }
}