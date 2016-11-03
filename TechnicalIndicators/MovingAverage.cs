namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class MovingAverage
    {
        #region Constants

        protected const int MaxNbOfItems = 5000;

        protected const int MaxPeriod = 200;

        #endregion

        #region Fields

        private readonly int period;

        private readonly List<decimal> sourceValues;

        private readonly List<decimal> values;

        #endregion

        #region Constructors and Destructors

        protected MovingAverage(int period)
        {
            if (period > MaxPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(period), $"The max allowed value is {MaxPeriod}");
            }

            this.period = period;
            this.sourceValues = new List<decimal>();
            this.values = new List<decimal>();
        }

        #endregion

        #region Public Properties

        public IEnumerable<decimal> SourceValues
        {
            get { return this.sourceValues; }
        }
        public IEnumerable<decimal> Values
        {
            get { return this.values; }
        }

        #endregion

        #region Properties

        protected int Period
        {
            get { return this.period; }
        }

        #endregion

        #region Public Methods and Operators

        public void Add(decimal value)
        {
            this.InternalAdd(value);
            this.EnsureMaxBufferSize();
        }

        public void AddRange(IEnumerable<decimal> inputValues)
        {
            if (inputValues == null)
            {
                throw new ArgumentNullException("sourceValues");
            }

            var valueList = inputValues.ToList();
            foreach (var value in valueList)
            {
                this.Add(value);
            }
        }

        #endregion

        #region Methods

        protected void AddSourceValue(decimal value)
        {
            this.sourceValues.Add(value);
        }

        protected void AddValue(decimal value)
        {
            this.values.Add(value);
        }

        protected void EnsureMaxBufferSize()
        {
            var nbItemsToRemove = this.SourceValues.Count() - MaxNbOfItems;
            if (nbItemsToRemove <= 0)
            {
                return;
            }

            this.sourceValues.RemoveRange(0, nbItemsToRemove);
            this.values.RemoveRange(0, nbItemsToRemove);
        }

        protected abstract void InternalAdd(decimal value);

        #endregion
    }
}