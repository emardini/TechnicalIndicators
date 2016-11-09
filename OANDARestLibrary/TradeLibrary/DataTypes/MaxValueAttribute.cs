namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    using System;

    public class MaxValueAttribute : Attribute
    {
        #region Constructors and Destructors

        public MaxValueAttribute(int i)
        {
            this.Value = i;
        }

        #endregion

        #region Public Properties

        public object Value { get; set; }

        #endregion
    }
}