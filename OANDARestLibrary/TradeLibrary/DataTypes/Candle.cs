namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public struct Candle
    {
        #region Public Properties

        public  decimal closeAsk { get; set; }

        public  decimal closeBid { get; set; }

        public  decimal closeMid { get; set; }

        public bool complete { get; set; }

        public  decimal highAsk { get; set; }

        // Bid/Ask candles

        public  decimal highBid { get; set; }

        public  decimal highMid { get; set; }

        public  decimal lowAsk { get; set; }

        public  decimal lowBid { get; set; }

        public  decimal lowMid { get; set; }

        public  decimal openAsk { get; set; }

        public  decimal openBid { get; set; }

        public  decimal openMid { get; set; }

        public string time { get; set; }

        public int volume { get; set; }

        #endregion
    }
}