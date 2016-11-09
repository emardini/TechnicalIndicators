namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications;

    public class Transaction : Response
    {
        #region Public Properties

        public double accountBalance { get; set; }

        public int accountId { get; set; }

        public string expiry { get; set; }

        public long id { get; set; }

        public string instrument { get; set; }

        public double interest { get; set; }

        public double lowerBound { get; set; }

        public long orderId { get; set; }

        public double pl { get; set; }

        public double price { get; set; }

        public string reason { get; set; }

        public string side { get; set; }

        public double stopLossPrice { get; set; }

        public double takeProfitPrice { get; set; }

        public string time { get; set; }

        public long tradeId { get; set; }

        public TradeData tradeOpened { get; set; }

        public TradeData tradeReduced { get; set; }

        public double trailingStopLossDistance { get; set; }

        public string type { get; set; }

        public int units { get; set; }

        public double upperBound { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetReadableString()
        {
            var readable = this.units + " " + this.instrument + " at " + this.price;
            if (this.pl != 0)
            {
                readable += "\nP/L: " + this.pl;
            }
            return readable;
        }

        /// <summary>
        ///     Gets a basic title for the type of transaction
        /// </summary>
        /// <returns></returns>
        public string GetTitle()
        {
            switch (this.type)
            {
                case "CloseOrder":
                    return "Order Closed";
                case "SellLimit":
                    return "Sell Limit Order Created";
                case "BuyLimit":
                    return "Buy Limit Order Created";
            }
            return this.type;
        }

        #endregion
    }
}