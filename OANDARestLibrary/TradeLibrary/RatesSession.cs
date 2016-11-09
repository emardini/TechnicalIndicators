namespace OANDARestLibrary.TradeLibrary
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using OANDARestLibrary.TradeLibrary.DataTypes;

    public class RatesSession : StreamSession<RateStreamResponse>
    {
        #region Fields

        private readonly List<Instrument> instruments;

        private readonly Rest proxy;

        #endregion

        #region Constructors and Destructors

        public RatesSession(int accountId, List<Instrument> instruments, Rest proxy) : base(accountId)
        {
            this.instruments = instruments;
            this.proxy = proxy;
        }

        #endregion

        #region Methods

        protected override async Task<WebResponse> GetSession()
        {
            return await proxy.StartRatesSession(this.instruments, this.AccountId);
        }

        #endregion
    }
}