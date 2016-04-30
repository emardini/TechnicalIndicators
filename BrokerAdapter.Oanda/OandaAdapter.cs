namespace BrokerAdapter.Oanda
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using OANDARestLibrary;
    using OANDARestLibrary.TradeLibrary.DataTypes;
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests;

    using TechnicalIndicators;

    using Candle = TechnicalIndicators.Candle;
    using Order = TechnicalIndicators.Order;

    public class OandaAdapter : IRateProvider, ITradingAdapter
    {
        private const string StatusHalted = "halted";

        #region Fields

        private readonly Rest proxy;

        #endregion

        #region Constructors and Destructors

        public OandaAdapter(string accountUrl, string ratesUrl, string streamingRatesUrl, string streamingEventsUrl, string labsUrl, string token)
        {
            this.proxy = new Rest(accountUrl, ratesUrl, streamingRatesUrl, streamingEventsUrl, labsUrl, token);
        }

        #endregion

        #region Public Methods and Operators

        public void CloseTrade(int accountId, long tradeId)
        {
            //TODO: Implement retries and notification in case the system cannot close the trade.
            //Which actions can it take if is not possible to close the order
            var response = this.proxy.DeleteTradeAsync(accountId, tradeId).Result;
        }

        public Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime = null)
        {
            return this.GetLastCandles(instrument, periodInMinutes, 1, endDateTime).FirstOrDefault();
        }

        public bool IsInstrumentHalted(string instrument)
        {
            var testPrice = this.GetLastPrices(instrument).FirstOrDefault();
            return testPrice == null || testPrice.status == StatusHalted;
        }

        public Trade GetOpenTrade(int accountId)
        {
            var response =
                this.proxy.GetTradeListAsync(accountId, new Dictionary<string, string> { { "count", "1" } }).Result;

            return
                response.Select(
                    x =>
                        new Trade
                        {
                            Id = x.id,
                            Instrument = x.instrument,
                            Price = (decimal)x.price,
                            Side = x.side,
                            StopLoss = (decimal)x.stopLoss,
                            TakeProfit = (decimal)x.takeProfit,
                            Time = x.time.SafeParseDate().GetValueOrDefault(),
                            TrailingAmount = (decimal)x.trailingAmount,
                            TrailingStop = x.trailingStop,
                            Units = x.units
                        }).FirstOrDefault();
        }

        public Rate GetRate(string instrument)
        {
            var rateResponse = this.GetLastPrices(instrument);
            var rate =
                rateResponse.Select(
                    x => new Rate { Ask = x.ask, Bid = x.bid, Instrument = instrument, Time = x.time.SafeParseDate().GetValueOrDefault() })
                    .FirstOrDefault();

            return rate;
        }

        private IEnumerable<Price> GetLastPrices(string instrument)
        {
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentException("Empty instrument");
            }

            instrument = instrument.Trim();
            if (instrument.Length != 7)
            {
                throw new ArgumentException(string.Format("Invalid instrument {0}", instrument));
            }

            var instrumentPar = GetInstrument(instrument);
            var rateResponse =
                this.proxy.GetRatesAsync(new List<Instrument> { instrumentPar })
                    .Result;
            return rateResponse;
        }

        public bool HasOpenOrder(int accountId)
        {
            var response =
                this.proxy.GetOrderListAsync(accountId, new Dictionary<string, string> { { "count", "1" } }).Result;

            return response.Any();
        }

        public bool HasOpenTrade(int accountId)
        {
            var response =
                this.proxy.GetTradeListAsync(accountId, new Dictionary<string, string> { { "count", "1" } }).Result;

            return response.Any();
        }

        public void PlaceOrder(Order order)
        {
            var result = this.proxy.PostOrderAsync(order.AcountId,
                new Dictionary<string, string>
                {
                    { "instrument", order.Instrument },
                    { "units", order.Units.ToString(CultureInfo.InvariantCulture) },
                    { "side", order.Side },
                    { "type", order.OrderType },
                    { "stopLoss", order.StopLoss.ToString(CultureInfo.InvariantCulture) }
                }).Result;
        }

        public void UpdateTrade(Trade updatedTrade)
        {
            var result = this.proxy.PatchTradeAsync(updatedTrade.AccountId,
                updatedTrade.Id,
                new Dictionary<string, string>
                {
                    { "takeProfit", updatedTrade.TakeProfit.ToString(CultureInfo.InvariantCulture) },
                    { "trailingStop", updatedTrade.TrailingStop.ToString(CultureInfo.InvariantCulture) },
                    { "stopLoss", updatedTrade.StopLoss.ToString(CultureInfo.InvariantCulture) }
                }).Result;
        }

        #endregion

        #region Explicit Interface Methods

        AccountInformation ITradingAdapter.GetAccountInformation(int accountId)
        {
            var response =
                this.proxy.GetAccountDetailsAsync(accountId).Result;

            return new AccountInformation { AccountId = response.accountId, AccountCurrency = response.accountCurrency, Balance = response.balance };
        }

        #endregion

        #region Methods

        private static EGranularity GetGranularity(int periodInMinutes)
        {
            switch (periodInMinutes)
            {
                case 5:
                    return EGranularity.M5;
                case 10:
                    return EGranularity.M10;
                case 15:
                    return EGranularity.M15;
                case 30:
                    return EGranularity.M30;
                case 60:
                    return EGranularity.H1;
                case 120:
                    return EGranularity.H2;
                case 180:
                    return EGranularity.H3;
                case 240:
                    return EGranularity.H4;
                case 360:
                    return EGranularity.H6;
                case 480:
                    return EGranularity.H8;
                case 720:
                    return EGranularity.H12;
                case 1440:
                    return EGranularity.D;
                case 10080:
                    return EGranularity.W;
                default:
                    return EGranularity.S5;
            }
        }

        private static Instrument GetInstrument(string instrument)
        {
            return new Instrument
            {
                instrument = instrument
            };
        }

        #endregion

        public IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime=null)
        {
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentException("Empty instrument");
            }

            instrument = instrument.Trim();
            if (instrument.Length != 7)
            {
                throw new ArgumentException(string.Format("Invalid instrument {0}", instrument));
            }

            var candleResponse =
                this.proxy.GetCandlesAsync(new CandlesRequest
                {
                    candleFormat = ECandleFormat.midpoint,
                    count = nbOfCandles+1,
                    granularity = GetGranularity(periodInMinutes),
                    instrument = instrument,
                    end = endDateTime.HasValue ? endDateTime.Value.ToUniversalTime().ToString("yy-MM-ddTHH:mm") : null
                }).Result;
            var candles =
                candleResponse.Where(x => x.complete)
                .OrderByDescending(x => x.time)
                .Select(x => new Candle(x.openMid, x.highMid, x.lowMid, x.closeMid, x.time.SafeParseDate().GetValueOrDefault()))
                .ToList();

            return candles;
        }
    }
}