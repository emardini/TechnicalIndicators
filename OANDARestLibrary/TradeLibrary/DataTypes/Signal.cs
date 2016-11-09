namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using System.Collections.Generic;

    public class Signal
    {
        #region Fields

        public Data data;

        public int id;

        public string instrument;

        public Metadata meta;

        public Prediction prediction;

        public string type;

        #endregion

        public class Data
        {
            #region Fields

            public long patternendtime;

            public Points points;

            public double price;

            #endregion
        }

        public class Metadata
        {
            #region Fields

            public int completed;

            public int direction;

            public HistoricalStats historicalstats;

            public int interval;

            public int length;

            public string pattern;

            public string patterntype;

            public double probability;

            public Scores scores;

            public string trendtype;

            #endregion
        }

        public class Point
        {
            #region Fields

            public long x0;

            public long x1;

            public double y0;

            public double y1;

            #endregion
        }

        public class Points
        {
            #region Fields

            public Dictionary<int, long> keytime;

            public Point resistance;

            public Point support;

            #endregion

            // Note: this doesn't appear to work
        }

        public class Prediction
        {
            #region Fields

            public double pricehigh;

            public double pricelow;

            public int timebars;

            public long timefrom;

            public long timeto;

            #endregion
        }

        public class Scores
        {
            #region Fields

            public int breakout;

            public int clarity;

            public int initialtrend;

            public int quality;

            public int uniformity;

            #endregion
        }
    }
}