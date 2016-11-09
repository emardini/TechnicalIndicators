namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class CandlesResponse
    {
        #region Fields

        public List<Candle> candles;

        public string granularity;

        public string instrument;

        #endregion
    }
}