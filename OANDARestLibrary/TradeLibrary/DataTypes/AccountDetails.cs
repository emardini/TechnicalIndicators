namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class AccountDetails
    {
        #region Fields

        public int accountId;

        public string homecurr;

        public double marginRate;

        public string name;

        public double nav;

        public int openOrders;

        public int openTrades;

        #endregion

        #region Public Properties

        public double balance { get; set; }

        public double marginAvail { get; set; }

        public double marginUsed { get; set; }

        public double realizedPl { get; set; }

        public double unrealizedPl { get; set; }

        #endregion
    }
}