namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Account
    {
        #region Fields

        public bool HasAccountCurrency;

        public bool HasAccountId;

        public bool HasAccountName;

        [IsOptional]
        public bool HasBalance;

        [IsOptional]
        public bool HasMarginAvail;

        public bool HasMarginRate;

        [IsOptional]
        public bool HasMarginUsed;

        [IsOptional]
        public bool HasOpenOrders;
        [IsOptional]
        public bool HasOpenTrades;
        [IsOptional]
        public bool HasRealizedPl;
        [IsOptional]
        public bool HasUnrealizedPl;

        private string _accountCurrency;

        private int _accountId;

        private string _accountName;

        private string _balance;

        private string _marginAvail;

        private string _marginRate;

        private string _marginUsed;

        private string _openOrders;

        private string _openTrades;

        private string _realizedPl;

        private string _unrealizedPl;

        #endregion

        #region Public Properties

        public string accountCurrency
        {
            get { return this._accountCurrency; }
            set
            {
                this._accountCurrency = value;
                this.HasAccountCurrency = true;
            }
        }
        public int accountId
        {
            get { return this._accountId; }
            set
            {
                this._accountId = value;
                this.HasAccountId = true;
            }
        }
        public string accountName
        {
            get { return this._accountName; }
            set
            {
                this._accountName = value;
                this.HasAccountName = true;
            }
        }
        public string balance
        {
            get { return this._balance; }
            set
            {
                this._balance = value;
                this.HasBalance = true;
            }
        }
        public string marginAvail
        {
            get { return this._marginAvail; }
            set
            {
                this._marginAvail = value;
                this.HasMarginAvail = true;
            }
        }
        public string marginRate
        {
            get { return this._marginRate; }
            set
            {
                this._marginRate = value;
                this.HasMarginRate = true;
            }
        }
        public string marginUsed
        {
            get { return this._marginUsed; }
            set
            {
                this._marginUsed = value;
                this.HasMarginUsed = true;
            }
        }

        public string openOrders
        {
            get { return this._openOrders; }
            set
            {
                this._openOrders = value;
                this.HasOpenOrders = true;
            }
        }
        public string openTrades
        {
            get { return this._openTrades; }
            set
            {
                this._openTrades = value;
                this.HasOpenTrades = true;
            }
        }
        public string realizedPl
        {
            get { return this._realizedPl; }
            set
            {
                this._realizedPl = value;
                this.HasRealizedPl = true;
            }
        }
        public string unrealizedPl
        {
            get { return this._unrealizedPl; }
            set
            {
                this._unrealizedPl = value;
                this.HasUnrealizedPl = true;
            }
        }

        #endregion
    }
}