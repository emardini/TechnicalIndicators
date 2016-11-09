namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using System;

    public class MinValueAttribute : Attribute
    {
        #region Constructors and Destructors

        public MinValueAttribute(int i)
        {
            this.Value = i;
        }

        #endregion

        #region Public Properties

        public object Value { get; set; }

        #endregion
    }
}