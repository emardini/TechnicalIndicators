namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
    public class OrderPostRequest
    {
        #region Fields

        public SmartProperty<string> expiry;

        public SmartProperty<string> instrument;

        [IsOptional]
        public SmartProperty<double> lowerBound;

        public SmartProperty<double> price;

        public SmartProperty<string> side;

        [IsOptional]
        public SmartProperty<double> stopLoss;

        [IsOptional]
        public SmartProperty<double> takeProfit;

        [IsOptional]
        public SmartProperty<double> trailingStop;

        public SmartProperty<string> type;

        public SmartProperty<int> units;

        [IsOptional]
        public SmartProperty<double> upperBound;

        #endregion
    }
}