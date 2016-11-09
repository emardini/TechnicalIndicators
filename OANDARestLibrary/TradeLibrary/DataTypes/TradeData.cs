namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications;

    public class TradeData : Response
    {
        #region Public Properties

        public long id { get; set; }

        public string instrument { get; set; }

        public double price { get; set; }

        public string side { get; set; }

        public double stopLoss { get; set; }

        public double takeProfit { get; set; }

        public string time { get; set; }

        public double trailingAmount { get; set; }

        public int trailingStop { get; set; }

        public int units { get; set; }

        #endregion
    }
}