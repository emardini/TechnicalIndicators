namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    public class DeleteTradeResponse : Response
    {
        #region Public Properties

        public long id { get; set; }

        public string instrument { get; set; }

        public double price { get; set; }

        public double profit { get; set; }

        public string side { get; set; }

        public string time { get; set; }

        #endregion
    }
}