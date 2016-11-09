namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using System;

    public class IsOptionalAttribute : Attribute
    {
        #region Public Methods and Operators

        public override string ToString()
        {
            return "Is Optional";
        }

        #endregion
    }
}