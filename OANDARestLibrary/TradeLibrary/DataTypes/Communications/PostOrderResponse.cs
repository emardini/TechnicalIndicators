namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class PostOrderResponse : Response
    {
        #region Public Properties

        public string instrument { get; set; }

        public Order orderOpened { get; set; }

        public double? price { get; set; }

        public string time { get; set; }

        public TradeData tradeOpened { get; set; }

        // TODO: verify this

        public Transaction tradeReduced { get; set; }

        public List<Transaction> tradesClosed { get; set; }

        #endregion

        // TODO: verify this
    }
}