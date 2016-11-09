namespace OANDARestLibrary.TradeLibrary
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using OANDARestLibrary.TradeLibrary.DataTypes;

    public class EventsSession : StreamSession<Event>
    {
        private readonly Rest proxy;

        #region Constructors and Destructors

        public EventsSession(int accountId, Rest proxy) : base(accountId)
        {
            this.proxy = proxy;
        }

        #endregion

        #region Methods

        protected override async Task<WebResponse> GetSession()
        {
            return await proxy.StartEventsSession(new List<int> { this.AccountId });
        }

        #endregion
    }
}