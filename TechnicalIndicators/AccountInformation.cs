namespace TechnicalIndicators
{
    public class AccountInformation
    {
        #region Fields

        public bool HasAccountCurrency;

        public bool HasAccountId;

        public bool HasAccountName;

        public bool HasBalance;

        public bool HasMarginAvail;

        public bool HasMarginRate;

        public bool HasMarginUsed;

        public bool HasOpenOrders;

        public bool HasOpenTrades;

        public bool HasRealizedPl;

        public bool HasUnrealizedPl;

        private string accountCurrency;

        private string accountId;

        private string accountName;

        private string balance;

        private string marginAvail;

        private string marginRate;

        private string marginUsed;

        private string openOrders;

        private string openTrades;

        private string realizedPl;

        private string unrealizedPl;

        #endregion

        #region Public Properties

        public string AccountCurrency
        {
            get { return this.accountCurrency; }
            set
            {
                this.accountCurrency = value;
                this.HasAccountCurrency = true;
            }
        }
        public string AccountId
        {
            get { return this.accountId; }
            set
            {
                this.accountId = value;
                this.HasAccountId = true;
            }
        }
        public string AccountName
        {
            get { return this.accountName; }
            set
            {
                this.accountName = value;
                this.HasAccountName = true;
            }
        }
        public string Balance
        {
            get { return this.balance; }
            set
            {
                this.balance = value;
                this.HasBalance = true;
            }
        }
        public string MarginAvail
        {
            get { return this.marginAvail; }
            set
            {
                this.marginAvail = value;
                this.HasMarginAvail = true;
            }
        }
        public string MarginRate
        {
            get { return this.marginRate; }
            set
            {
                this.marginRate = value;
                this.HasMarginRate = true;
            }
        }
        public string MarginUsed
        {
            get { return this.marginUsed; }
            set
            {
                this.marginUsed = value;
                this.HasMarginUsed = true;
            }
        }

        public string OpenOrders
        {
            get { return this.openOrders; }
            set
            {
                this.openOrders = value;
                this.HasOpenOrders = true;
            }
        }
        public string OpenTrades
        {
            get { return this.openTrades; }
            set
            {
                this.openTrades = value;
                this.HasOpenTrades = true;
            }
        }
        public string RealizedPl
        {
            get { return this.realizedPl; }
            set
            {
                this.realizedPl = value;
                this.HasRealizedPl = true;
            }
        }
        public string UnrealizedPl
        {
            get { return this.unrealizedPl; }
            set
            {
                this.unrealizedPl = value;
                this.HasUnrealizedPl = true;
            }
        }

        #endregion
    }
}