namespace TechnicalIndicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class MovingAverage
    {
        protected MovingAverage(int period)
        {
            if (period > MaxPeriod)
            {
                throw new ArgumentOutOfRangeException("period", string.Format("The max allowed value is {0}", MaxPeriod));
            }

            this.period = period;
            this.sourceValues = new List<decimal>();
            this.values = new List<decimal>();
        }

        protected const int MaxNbOfItems = 5000;

        protected const int MaxPeriod = 200;

        private readonly int period;

        protected int Period
        {
            get { return period; }
        }
      
        private readonly List<decimal> values;
        public IEnumerable<decimal> Values
        {
            get { return this.values; }
        }

        private readonly List<decimal> sourceValues;
        public IEnumerable<decimal> SourceValues
        {
            get { return this.sourceValues; }
        }

        protected void AddSourceValue(decimal value)
        {
            sourceValues.Add(value);
        }

        protected void EnsureMaxBufferSize()
        {
            if (SourceValues.Count() <= MaxNbOfItems)
            {
                return;
            }
            this.sourceValues.RemoveAt(0);
            this.values.RemoveAt(0);
        }

        protected void AddValue(decimal value)
        {
            values.Add(value);
        }

        protected abstract void InternalAdd(decimal value);

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
    }
}