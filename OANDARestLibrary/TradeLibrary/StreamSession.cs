namespace OANDARestLibrary.TradeLibrary
{
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;

    using OANDARestLibrary.TradeLibrary.DataTypes;

    public abstract class StreamSession<T> where T : IHeartbeat
    {
        #region Fields

        protected readonly int AccountId;

        private WebResponse response;

        private bool shutdown;

        #endregion

        #region Constructors and Destructors

        protected StreamSession(int accountId)
        {
            this.AccountId = accountId;
        }

        #endregion

        #region Delegates

        public delegate void DataHandler(T data);

        #endregion

        #region Public Events

        public event DataHandler DataReceived;

        #endregion

        #region Public Methods and Operators

        public void OnDataReceived(T data)
        {
            var handler = this.DataReceived;
            if (handler != null)
            {
                handler(data);
            }
        }

        public async void StartSession()
        {
            this.shutdown = false;
            this.response = await this.GetSession();

            await Task.Run(() =>
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                var reader = new StreamReader(this.response.GetResponseStream());
                while (!this.shutdown)
                {
                    var memStream = new MemoryStream();

                    var line = reader.ReadLine();
                    memStream.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetByteCount(line));
                    memStream.Position = 0;

                    var data = (T)serializer.ReadObject(memStream);

                    // Don't send heartbeats
                    if (!data.IsHeartbeat())
                    {
                        this.OnDataReceived(data);
                    }
                }
            }
                );
        }

        public void StopSession()
        {
            this.shutdown = true;
        }

        #endregion

        #region Methods

        protected abstract Task<WebResponse> GetSession();

        #endregion
    }
}