namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Position
    {
        #region Public Properties

        public double avgPrice { get; set; }

        public string instrument { get; set; }

        public string side { get; set; }

        public int units { get; set; }

        #endregion
    }
}