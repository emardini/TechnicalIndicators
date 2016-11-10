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

        public decimal stopLoss { get; set; }

        public decimal takeProfit { get; set; }

        public string time { get; set; }

        public decimal trailingAmount { get; set; }

        public decimal trailingStop { get; set; }

        public int units { get; set; }

        #endregion
    }
}