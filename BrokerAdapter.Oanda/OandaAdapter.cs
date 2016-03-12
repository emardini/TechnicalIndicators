﻿namespace BrokerAdapter.Oanda
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OANDARestLibrary;
    using OANDARestLibrary.TradeLibrary.DataTypes;
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests;

    using TechnicalIndicators;

    using Candle = TechnicalIndicators.Candle;

    public class OandaAdapter : IRateProvider, ITradingAdapter
    {
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
            var result = this.proxy.DeleteTradeAsync(accountId, tradeId).Result;
        }

        public Candle GetLastCandle(string instrument, int periodInMinutes)
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
                    count = 1,
                    granularity = GetGranularity(periodInMinutes),
                    instrument = instrument
                }).Result;
            var candle =
                candleResponse.Select(x => new Candle(x.openMid, x.highMid, x.lowMid, x.closeMid, x.time.SafeParseDate().GetValueOrDefault()))
                    .FirstOrDefault();

            return candle;
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
            var candle =
                rateResponse.Select(
                    x => new Rate { Ask = x.ask, Bid = x.bid, Instrument = instrument, Time = x.time.SafeParseDate().GetValueOrDefault() })
                    .FirstOrDefault();

            return candle;
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

        #endregion

        #region Methods

        private static EGranularity GetGranularity(int periodInMinutes)
        {
            return EGranularity.M5;
        }

        private static Instrument GetInstrument(string instrument)
        {
            return new Instrument
            {
                instrument = instrument
            };
        }

        #endregion
    }
}