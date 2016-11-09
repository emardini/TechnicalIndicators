namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
    internal abstract class AccountRequest : Request
    {
        #region Fields

        private readonly int _accountId;

        #endregion

        #region Constructors and Destructors

        private AccountRequest(int accountId)
        {
            this._accountId = accountId;
        }

        #endregion

        #region Public Properties

        public override string EndPoint
        {
            get { return "/accounts/" + this._accountId + this.GetAccountEndPoint(); }
        }

        #endregion

        #region Public Methods and Operators

        public override EServer GetServer()
        {
            return EServer.Account;
        }

        #endregion

        #region Methods

        protected abstract string GetAccountEndPoint();

        #endregion
    }
}