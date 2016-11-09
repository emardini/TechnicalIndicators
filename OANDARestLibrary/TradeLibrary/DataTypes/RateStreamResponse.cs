namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class RateStreamResponse : IHeartbeat
    {
        #region Fields

        public Heartbeat heartbeat;

        public Price tick;

        #endregion

        #region Public Methods and Operators

        public bool IsHeartbeat()
        {
            return (this.heartbeat != null);
        }

        #endregion
    }
}