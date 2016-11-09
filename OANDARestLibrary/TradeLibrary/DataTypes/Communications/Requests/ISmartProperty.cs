namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
    public interface ISmartProperty
    {
        #region Public Properties

        bool HasValue { get; set; }

        #endregion

        #region Public Methods and Operators

        void SetValue(object obj);

        #endregion
    }
}