namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class TransactionsResponse : Response
    {
        #region Fields

        public string nextPage;

        public List<Transaction> transactions;

        #endregion
    }
}