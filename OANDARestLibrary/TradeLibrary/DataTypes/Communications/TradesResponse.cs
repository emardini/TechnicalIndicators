namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class TradesResponse
    {
        #region Fields

        public string nextPage;

        public List<TradeData> trades;

        #endregion
    }
}