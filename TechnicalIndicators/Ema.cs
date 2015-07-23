namespace TechnicalIndicators
{
    using System.Linq;

    public class Ema : MovingAverage
    {
        #region Fields

        private readonly decimal smoothingFactor;

        #endregion

        #region Constructors and Destructors

        public Ema(int period) : base(period)
        {
            this.smoothingFactor = 2m / (this.Period + 1m);
        }

        #endregion

        #region Methods

        protected override void InternalAdd(decimal value)
        {
            this.AddSourceValue(value);
            var newValue = SourceValues.Count() == 1 ? value :  value * this.smoothingFactor + this.Values.LastOrDefault() * (1m - this.smoothingFactor);
            this.AddValue(newValue);
        }

        #endregion
    }
}