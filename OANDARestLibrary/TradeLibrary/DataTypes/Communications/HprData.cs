namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;

    public class HprData
    {
        #region Fields

        public List<List<string>> data;

        public string label;

        #endregion

        #region Public Methods and Operators

        public List<HistoricalPositionRatio> GetData()
        {
            var result = new List<HistoricalPositionRatio>();
            foreach (var list in this.data)
            {
                var hpr = new HistoricalPositionRatio
                {
                    exchangeRate = double.Parse(list[2]),
                    longPositionRatio = double.Parse(list[1]),
                    timestamp = long.Parse(list[0])
                };
                result.Add(hpr);
            }
            return result;
        }

        #endregion
    }
}