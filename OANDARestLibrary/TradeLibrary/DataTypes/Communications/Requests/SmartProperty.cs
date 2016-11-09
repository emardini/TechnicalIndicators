namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
    public struct SmartProperty<T> : ISmartProperty
    {
        #region Fields

        private T _value;

        #endregion

        #region Public Properties

        public bool HasValue { get; set; }

        public T Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.HasValue = true;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static implicit operator SmartProperty<T>(T value)
        {
            return new SmartProperty<T> { Value = value };
        }

        public static implicit operator T(SmartProperty<T> value)
        {
            return value._value;
        }

        public void SetValue(object obj)
        {
            this.SetValue((T)obj);
        }

        public void SetValue(T value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            // This is ugly, but c'est la vie for now
            if (this._value is bool)
            {
                // bool values need to be lower case to be parsed correctly
                return this._value.ToString().ToLower();
            }
            return this._value.ToString();
        }

        #endregion
    }
}