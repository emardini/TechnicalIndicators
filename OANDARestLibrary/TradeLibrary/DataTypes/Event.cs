namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Event : IHeartbeat
    {
        #region Public Properties

        public Heartbeat heartbeat { get; set; }

        public Transaction transaction { get; set; }

        #endregion

        #region Public Methods and Operators

        public bool IsHeartbeat()
        {
            return (this.heartbeat != null);
        }

        #endregion
    }
}