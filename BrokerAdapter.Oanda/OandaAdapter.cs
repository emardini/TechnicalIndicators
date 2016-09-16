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
        #region Constants

        private const string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";

        private const int MinInstrumentLenght = 7;

        private const string StatusHalted = "halted";

        #endregion

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

        public virtual void CloseTrade(string accountId, long tradeId)
        {
            //TODO: Implement retries and notification in case the system cannot close the trade.
            //Which actions can it take if is not possible to close the order
            var response = this.proxy.DeleteTradeAsync(accountId, tradeId).Result;
        }

        public virtual AccountInformation GetAccountInformation(string accountId)
        {
            var response =
                this.proxy.GetAccountDetailsAsync(accountId).Result;

            return new AccountInformation { AccountId = response.accountId, AccountCurrency = response.accountCurrency, Balance = response.balance, MarginRate = response.marginRate};
        }

        public virtual Candle GetLastCandle(string instrument, int periodInMinutes, DateTime? endDateTime = null)
        {
            return this.GetLastCandles(instrument, periodInMinutes, 1, endDateTime).FirstOrDefault();
        }

        public virtual IEnumerable<Candle> GetLastCandles(string instrument, int periodInMinutes, int nbOfCandles, DateTime? endDateTime = null)
        {
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentException("Empty instrument");
            }

            instrument = instrument.Trim();
            if (instrument.Length != MinInstrumentLenght)
            {
                throw new ArgumentException(string.Format("Invalid instrument {0}", instrument));
            }

            List<Candle> candles;
            try
            {                
                List<OANDARestLibrary.TradeLibrary.DataTypes.Candle> candleResponse;
                if (endDateTime.HasValue)
                {
                    var endTimeRef = EscapeDateValue(endDateTime.Value);
                    var startTimeRef = EscapeDateValue(endDateTime.Value.AddMinutes(-nbOfCandles * periodInMinutes - 1));

                    candleResponse = this.proxy.GetCandlesAsync(new CandlesRequest
                    {
                        candleFormat = ECandleFormat.midpoint,
                        granularity = GetGranularity(periodInMinutes),
                        instrument = instrument,
                        includeFirst = true,
                        end = endTimeRef,
                        start = startTimeRef
                    }).Result;
                }
                else
                {
                    var includeFirstRef = new SmartProperty<bool>();
                    candleResponse = this.proxy.GetCandlesAsync(new CandlesRequest
                    {
                        candleFormat = ECandleFormat.midpoint,
                        granularity = GetGranularity(periodInMinutes),
                        instrument = instrument,
                        includeFirst = includeFirstRef,
                        count = nbOfCandles + 1,
                    }).Result;
                }

                candles = candleResponse.Where(x => x.complete)
                    .OrderBy(x => x.time)
                    .Select(x => new Candle(x.openMid, x.highMid, x.lowMid, x.closeMid, x.time.SafeParseDate().GetValueOrDefault()))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return candles;
        }

        private static string EscapeDateValue(DateTime thatTime)
        {
            return Uri.EscapeDataString(thatTime.ToUniversalTime().ToString(DateFormat));
        }

        public virtual Trade GetOpenTrade(string accountId)
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

        public virtual Rate GetRate(string instrument)
        {
            var rateResponse = this.GetLastPrices(instrument);
            var rate =
                rateResponse.Select(
                    x => new Rate { Ask = x.ask, Bid = x.bid, Instrument = instrument, Time = x.time.SafeParseDate().GetValueOrDefault() })
                    .FirstOrDefault();

            return rate;
        }

        public virtual bool HasOpenOrder(string accountId)
        {
            var response =
                this.proxy.GetOrderListAsync(accountId, new Dictionary<string, string> { { "count", "1" } }).Result;

            return response.Any();
        }

        public virtual bool HasOpenTrade(string accountId)
        {
            var response =
                this.proxy.GetTradeListAsync(accountId, new Dictionary<string, string> { { "count", "1" } }).Result;

            return response.Any();
        }

        public virtual bool IsInstrumentHalted(string instrument)
        {
            var testPrice = this.GetLastPrices(instrument).FirstOrDefault();
            return testPrice == null || testPrice.status == StatusHalted;
        }

        public virtual void PlaceOrder(Order order)
        {
            var result = this.proxy.PostOrderAsync(order.AcountId,
                new Dictionary<string, string>
                {
                    { "instrument", order.Instrument },
                    { "units", order.Units.ToString(CultureInfo.InvariantCulture) },
                    { "side", order.Side },
                    { "type", order.OrderType },
                    { "stopLoss", order.StopLoss.ToString(CultureInfo.InvariantCulture) },
                    { "trailingStop", string.Format("{0:0.0}", order.TrailingStop) }
                }).Result;
        }

        public virtual void UpdateTrade(Trade updatedTrade)
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

        private IEnumerable<Price> GetLastPrices(string instrument)
        {
            if (string.IsNullOrWhiteSpace(instrument))
            {
                throw new ArgumentException("Empty instrument");
            }

            instrument = instrument.Trim();
            if (instrument.Length != MinInstrumentLenght)
            {
                throw new ArgumentException(string.Format("Invalid instrument {0}", instrument));
            }

            var instrumentPar = GetInstrument(instrument);
            var rateResponse =
                this.proxy.GetRatesAsync(new List<Instrument> { instrumentPar })
                    .Result;
            return rateResponse;
        }

        #endregion
    }
}