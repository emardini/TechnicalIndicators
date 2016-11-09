namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Price
    {
        #region Fields

        public State state = State.Default;

        public string status;

        public string time;

        #endregion

        #region Enums

        public enum State
        {
            Default,

            Increasing,

            Decreasing
        };

        #endregion

        #region Public Properties

        public decimal ask { get; set; }

        public decimal bid { get; set; }

        public string instrument { get; set; }

        #endregion

        #region Public Methods and Operators

        public void update(Price update)
        {
            if (this.bid > update.bid)
            {
                this.state = State.Decreasing;
            }
            else if (this.bid < update.bid)
            {
                this.state = State.Increasing;
            }
            else
            {
                this.state = State.Default;
            }

            this.bid = update.bid;
            this.ask = update.ask;
            this.time = update.time;
        }

        #endregion
    }
}