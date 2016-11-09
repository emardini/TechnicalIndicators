namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    using System.Collections.Generic;
    using System.Reflection;

    public class CommitmentsOfTradersResponse
    {
        #region Fields

        public List<CommitmentsOfTraders> AUD_USD;

        public List<CommitmentsOfTraders> EUR_USD;

        public List<CommitmentsOfTraders> GBP_USD;

        public List<CommitmentsOfTraders> NZD_USD;

        public List<CommitmentsOfTraders> USD_CAD;

        public List<CommitmentsOfTraders> USD_CHF;

        public List<CommitmentsOfTraders> USD_JPY;

        public List<CommitmentsOfTraders> USD_MXN;

        public List<CommitmentsOfTraders> XAG_USD;

        public List<CommitmentsOfTraders> XAU_USD;

        #endregion

        #region Public Methods and Operators

        public List<CommitmentsOfTraders> GetData()
        {
            // Built in assumption, there's only one HprData in this object (since we can only request data for one instrument at a time)
            foreach (var field in typeof(CommitmentsOfTradersResponse).GetTypeInfo().DeclaredFields)
            {
                var cotData = (List<CommitmentsOfTraders>)field.GetValue(this);
                if (cotData != null)
                {
                    return cotData;
                }
            }
            return null;
        }

        #endregion
    }
}