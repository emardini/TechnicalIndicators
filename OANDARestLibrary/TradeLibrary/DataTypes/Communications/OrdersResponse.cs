namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class OrdersResponse
    {
        #region Fields

        public string nextPage;

        public List<Order> orders;

        #endregion
    }
}