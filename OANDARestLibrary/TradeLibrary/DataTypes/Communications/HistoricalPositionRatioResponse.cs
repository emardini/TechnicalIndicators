namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;
    using System.Reflection;

    public class HistoricalPositionRatioResponse
    {
        #region Fields

        public InnerHprResponse data;

        #endregion

        #region Public Methods and Operators

        public List<HistoricalPositionRatio> GetData()
        {
            // Built in assumption, there's only one HprData in this object (since we can only request data for one instrument at a time)
            foreach (var field in typeof(InnerHprResponse).GetTypeInfo().DeclaredFields)
            {
                var hprData = (HprData)field.GetValue(this.data);
                if (hprData != null)
                {
                    return hprData.GetData();
                }
            }
            return null;
        }

        #endregion
    }
}