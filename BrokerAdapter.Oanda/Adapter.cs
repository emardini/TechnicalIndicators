namespace BrokerAdapter.Oanda
{
    using System;
    using System.Linq;

    using OANDARestLibrary;
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests;

    using TechnicalIndicators;

    public class Adapter : IRateProvider
    {
        #region Fields

        private readonly Rest proxy;

        #endregion

        #region Constructors and Destructors

        public Adapter(string accountUrl, string ratesUrl, string streamingRatesUrl, string streamingEventsUrl, string labsUrl, string token)
        {
            this.proxy = new Rest(accountUrl, ratesUrl, streamingRatesUrl, streamingEventsUrl, labsUrl, token);
        }

        #endregion

        #region Public Methods and Operators

        public Candle GetLastCandle(string instrument, int periodInMinutes, int accountId)
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
            var candle = candleResponse.Select(x => new Candle(x.openMid, x.highMid, x.lowMid, x.closeMid)).FirstOrDefault();

            return candle;
        }

        #endregion

        #region Methods

        private static EGranularity GetGranularity(int periodInMinutes)
        {
            return EGranularity.M5;
        }

        #endregion

        public Rate GetRate(string instrument)
        {
            throw new NotImplementedException();
        }
    }
}