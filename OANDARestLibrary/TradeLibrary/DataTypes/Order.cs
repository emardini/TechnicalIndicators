namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications;

    public class Order : Response
    {
        #region Public Properties

        public string expiry { get; set; }

        public long id { get; set; }

        public string instrument { get; set; }

        public double lowerBound { get; set; }

        public decimal price { get; set; }

        public string side { get; set; }

        public decimal stopLoss { get; set; }

        public decimal takeProfit { get; set; }

        public string time { get; set; }

        public decimal trailingStop { get; set; }

        public string type { get; set; }

        public int units { get; set; }

        public double upperBound { get; set; }

        #endregion
    }
}