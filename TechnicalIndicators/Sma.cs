namespace TechnicalIndicators
{
    using System.Linq;

    public class Sma : MovingAverage
    {
        #region Constructors and Destructors

        public Sma(int period) : base(period)
        {
        }

        #endregion

        #region Methods

        protected override void InternalAdd(decimal value)
        {
            this.AddSourceValue(value);
            var newSma = this.SourceValues.TakeLast(this.Period).Average();
            this.AddValue(newSma);
        }

        #endregion
    }
}