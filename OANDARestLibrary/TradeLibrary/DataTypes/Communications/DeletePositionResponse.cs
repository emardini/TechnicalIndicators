namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class DeletePositionResponse : Response
    {
        #region Public Properties

        public List<long> ids { get; set; }

        public string instrument { get; set; }

        public double price { get; set; }

        public int totalUnits { get; set; }

        #endregion
    }
}