namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SpreadsResponse
    {
        #region Fields

        public List<List<string>> avg;

        public List<List<string>> max;

        public List<List<string>> min;

        #endregion

        #region Public Methods and Operators

        public List<SpreadPeriod> GetData()
        {
            var results = new SortedDictionary<long, SpreadPeriod>();
            this.AddValues(results, this.max, (x, y) => x.max = y);
            this.AddValues(results, this.min, (x, y) => x.min = y);
            this.AddValues(results, this.avg, (x, y) => x.avg = y);

            double lastMax = 0;
            double lastMin = 0;
            double lastAvg = 0;
            foreach (var period in results.Select(pair => pair.Value))
            {
                if (period.max == 0)
                {
                    period.max = lastMax;
                }
                if (period.min == 0)
                {
                    period.min = lastMin;
                }
                if (period.avg == 0)
                {
                    period.avg = lastAvg;
                }
                lastMax = period.max;
                lastMin = period.min;
                lastAvg = period.avg;
            }

            return results.Values.ToList();
        }

        #endregion

        #region Methods

        private void AddValues(SortedDictionary<long, SpreadPeriod> results, List<List<string>> valueList, Func<SpreadPeriod, double, double> setValue)
        {
            foreach (var list in valueList)
            {
                var timestamp = Convert.ToInt64(list[0]);
                var value = Convert.ToDouble(list[1]);

                if (!results.ContainsKey(timestamp))
                {
                    results[timestamp] = new SpreadPeriod { timestamp = timestamp };
                }
                setValue(results[timestamp], value);
            }
        }

        #endregion
    }
}