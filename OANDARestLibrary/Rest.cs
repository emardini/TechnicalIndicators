namespace OANDARestLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;

    using OANDARestLibrary.TradeLibrary.DataTypes;
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications;
    using OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests;

    /// Best Practices Notes
    /// 
    /// Keep alive is on by default
    public class Rest
    {
        // Convenience helpers

        #region Fields

        private readonly string accessToken;

        private readonly string accountUrl;

        private readonly string labsUrl;

        private readonly string ratesUrl;

        private readonly string streamingEventsUrl;

        private readonly string streamingRatesUrl;

        #endregion

        #region Constructors and Destructors

        public Rest(string accountUrl,
            string ratesUrl,
            string streamingRatesUrl,
            string streamingEventsurl,
            string labsUrl,
            string accessToken)
        {
            this.accountUrl = accountUrl;
            this.ratesUrl = ratesUrl;
            this.streamingRatesUrl = streamingRatesUrl;
            this.streamingEventsUrl = streamingEventsurl;
            this.accessToken = accessToken;
            this.labsUrl = labsUrl;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Note: This only works against the sandbox server
        /// </summary>
        /// <returns>The response with the account details</returns>
        public async Task<AccountResponse> CreateAccount()
        {
            var requestString = this.accountUrl + "accounts";

            return await this.MakeRequestAsync<AccountResponse>(requestString, "POST");
        }

        /// <summary>
        ///     Delete the order specified
        /// </summary>
        /// <param name="accountId">the account that owns the order</param>
        /// <param name="orderId">the ID of the order to delete</param>
        /// <returns>Order object containing the details of the deleted order</returns>
        public async Task<Order> DeleteOrderAsync(int accountId, long orderId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/orders/" + orderId;
            return await this.MakeRequestAsync<Order>(requestString, "DELETE");
        }

        /// <summary>
        ///     Close the given position
        ///     This will close all trades on the provided account/instrument
        /// </summary>
        /// <param name="accountId">the account to close trades on</param>
        /// <param name="instrument">the instrument for which to close all trades</param>
        /// <returns>DeletePositionResponse object containing details about the actions taken</returns>
        public async Task<DeletePositionResponse> DeletePositionAsync(int accountId, string instrument)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/positions/" + instrument;

            return await this.MakeRequestAsync<DeletePositionResponse>(requestString, "DELETE");
        }

        /// <summary>
        ///     Close the trade specified
        /// </summary>
        /// <param name="accountId">the account that owns the trade</param>
        /// <param name="tradeId">the ID of the trade to close</param>
        /// <returns>DeleteTradeResponse containing the details of the close</returns>
        public async Task<DeleteTradeResponse> DeleteTradeAsync(int accountId, long tradeId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/trades/" + tradeId;
            return await this.MakeRequestAsync<DeleteTradeResponse>(requestString, "DELETE");
        }

        /// <summary>
        ///     Retrieves the details for a given account
        /// </summary>
        /// <param name="accountId">details will be retrieved for this account id</param>
        /// <returns>Account object containing the account details</returns>
        public async Task<Account> GetAccountDetailsAsync(int accountId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId;

            var accountDetails = await this.MakeRequestAsync<Account>(requestString);
            return accountDetails;
        }

        /// <summary>
        ///     Retrieves all the accounts belonging to the user
        /// </summary>
        /// <param name="user">the username to use -- only needed on sandbox-- otherwise identified by the token used</param>
        /// <returns>list of accounts, including basic information about the accounts</returns>
        public async Task<List<Account>> GetAccountListAsync(string user = "")
        {
            var requestString = this.accountUrl + "accounts";
            if (!string.IsNullOrEmpty(user))
            {
                requestString += "?username=" + user;
            }

            var result = await this.MakeRequestAsync<AccountsResponse>(requestString);
            return result.accounts;
        }

        /// <summary>
        ///     Retrieves autochartist signals
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="requestParameters"> optional parameters for the request, refer to the full doc for details </param>
        /// <returns>List of CalendarEvent objects</returns>
        public async Task<List<Signal>> GetAutochartistData(Dictionary<string, string> requestParameters = null)
        {
            var requestString = this.labsUrl + "" + "signal/autochartist";

            var response = await this.MakeRequestAsync<AutochartistResponse>(requestString, "GET", requestParameters);

            return response.signals;
        }

        /// <summary>
        ///     Retrieves calendar data for the instrument and period provided
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="instrument">the instrument for which to get data (any tradable instrument)</param>
        /// <param name="period">
        ///     the period, in seconds, for which to get data (1 hour (3600) to 1 year (31536000), will be
        ///     adjusted to nearest valid value, see full docs)
        /// </param>
        /// <returns>List of CalendarEvent objects</returns>
        public async Task<List<CalendarEvent>> GetCalendarData(string instrument, int period)
        {
            var requestString = this.labsUrl + "" + "calendar?instrument=" + instrument + "&period=" + period;

            return await this.MakeRequestAsync<List<CalendarEvent>>(requestString);
        }

        /// <summary>
        ///     Basic request to retrieve candles for the pair/granularity provided
        /// </summary>
        /// <param name="pair">The pair/symbol for which to retrieve candles</param>
        /// <param name="granularity">the granularity for which to retrieve candles</param>
        /// <returns>List of Candle objects (or empty list)</returns>
        public async Task<List<Candle>> GetCandlesAsync(string pair, string granularity)
        {
            var requestString = this.ratesUrl + "" + "instruments/" + pair + "/candles?granularity=" + granularity;

            var candlesResponse = await this.MakeRequestAsync<CandlesResponse>(requestString);
            var candles = new List<Candle>();
            candles.AddRange(candlesResponse.candles);

            return candles;
        }

        /// <summary>
        ///     More detailed request to retrieve candles
        /// </summary>
        /// <param name="request">the request data to use when retrieving the candles</param>
        /// <returns>List of Candles received (or empty list)</returns>
        public async Task<List<Candle>> GetCandlesAsync(CandlesRequest request)
        {
            var requestString = this.ratesUrl + request.GetRequestString();

            var candlesResponse = await this.MakeRequestAsync<CandlesResponse>(requestString);
            var candles = new List<Candle>();
            candles.AddRange(candlesResponse.candles);

            return candles;
        }

        /// <summary>
        ///     Retrieves commitment of traders data for the instrument and period provided
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="instrument">
        ///     the instrument for which to get data (subset of instruments supported, mostly majors, see full
        ///     doc)
        /// </param>
        /// <returns>List of CalendarEvent objects</returns>
        public async Task<List<CommitmentsOfTraders>> GetCommitmentOfTradersData(string instrument)
        {
            var requestString = this.labsUrl + "" + "commitments_of_traders?instrument=" + instrument;

            var response = await this.MakeRequestAsync<CommitmentsOfTradersResponse>(requestString);

            return response.GetData();
        }

        /// <summary>
        ///     Expensive request to retrieve the entire transaction history for a given account
        ///     This request may take some time
        ///     This request is heavily rate limited
        ///     This request does not work on sandbox
        /// </summary>
        /// <param name="accountId">the id of the account for which to retrieve the history</param>
        /// <returns>List of Transaction objects with the details of all transactions</returns>
        public async Task<List<Transaction>> GetFullTransactionHistoryAsync(int accountId)
        {
            // NOTE: this does not work on sandbox
            var requestString = this.accountUrl + "accounts/" + accountId + "/alltransactions";

            var request = WebRequest.CreateHttp(requestString);
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;
            request.Method = "GET";
            string location;
            // Phase 1: request and get the location
            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    location = response.Headers["Location"];
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var stream = new StreamReader(response.GetResponseStream());
                var result = stream.ReadToEnd();
                throw new Exception(result);
            }

            // Phase 2: wait for and retrieve the actual data
            var client = new HttpClient();

            //request = WebRequest.CreateHttp(location);
            for (var retries = 0; retries < 20; retries++)
            {
                try
                {
                    var response = await client.GetAsync(location);
                    if (response.IsSuccessStatusCode)
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<Transaction>));
                        var archive = new ZipArchive(await response.Content.ReadAsStreamAsync());
                        return (List<Transaction>)serializer.ReadObject(archive.Entries[0].Open());
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Not found is expected until the resource is ready
                        // Delay a bit to wait for the response
                        await Task.Delay(500);
                    }
                    else
                    {
                        var stream = new StreamReader(await response.Content.ReadAsStreamAsync());
                        var result = stream.ReadToEnd();
                        throw new Exception(result);
                    }
                }
                catch (WebException ex)
                {
                    var response = (HttpWebResponse)ex.Response;
                    var stream = new StreamReader(response.GetResponseStream());
                    var result = stream.ReadToEnd();
                    throw new Exception(result);
                }
            }
            return null;
        }

        /// <summary>
        ///     Retrieves historical position ratio data for the instrument and period provided
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="instrument">the instrument for which to get data (limited subset, mostly majors, see full docs)</param>
        /// <param name="period">
        ///     the period, in seconds, for which to get data (1 day (86400) to 1 year (31536000), will be adjusted to nearest
        ///     valid value, see full docs)
        ///     Note that the longer the period requested, the more spaced out the snapshots will be
        /// </param>
        /// <returns>List of CalendarEvent objects</returns>
        public async Task<List<HistoricalPositionRatio>> GetHistoricalPostionRatioData(string instrument, int period)
        {
            var requestString = this.labsUrl + "" + "historical_position_ratios?instrument=" + instrument + "&period=" + period;

            var response = await this.MakeRequestAsync<HistoricalPositionRatioResponse>(requestString);

            return response.GetData();
        }

        /// <summary>
        ///     Retrieves the list of instruments available for the given account
        /// </summary>
        /// <param name="account">the account to check</param>
        /// <param name="fields">optional - the fields to request in the response</param>
        /// <param name="instrumentNames">optional - the instruments to request details for</param>
        /// <returns>List of Instrument objects with details about each instrument</returns>
        public async Task<List<Instrument>> GetInstrumentsAsync(int account, List<string> fields = null, List<string> instrumentNames = null)
        {
            var requestString = this.ratesUrl + "instruments?accountId=" + account;

            if (fields != null)
            {
                var fieldsParam = string.Join(",", fields);
                requestString += "&fields=" + Uri.EscapeDataString(fieldsParam);
            }
            if (instrumentNames != null)
            {
                var instrumentsParam = string.Join(",", instrumentNames);
                requestString += "&instruments=" + Uri.EscapeDataString(instrumentsParam);
            }

            var instrumentResponse = await this.MakeRequestAsync<InstrumentsResponse>(requestString);

            var instruments = new List<Instrument>();
            instruments.AddRange(instrumentResponse.instruments);

            return instruments;
        }

        /// <summary>
        ///     Retrieves the details for a given order ID
        /// </summary>
        /// <param name="accountId">the account that the order belongs to</param>
        /// <param name="orderId">the id of the order to retrieve</param>
        /// <returns>Order object containing the order details</returns>
        public async Task<Order> GetOrderDetailsAsync(int accountId, long orderId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/orders/" + orderId;

            var order = await this.MakeRequestAsync<Order>(requestString);

            return order;
        }

        /// <summary>
        ///     Retrieves the list of open orders belonging to the account
        /// </summary>
        /// <param name="account">the account to retrieve the list for</param>
        /// <param name="requestParams">optional additional parameters for the request (name, value pairs)</param>
        /// <returns>List of Order objects (or empty list, if no orders)</returns>
        public async Task<List<Order>> GetOrderListAsync(int account, Dictionary<string, string> requestParams = null)
        {
            var requestString = this.accountUrl + "accounts/" + account + "/orders";

            var ordersResponse = await this.MakeRequestAsync<OrdersResponse>(requestString, "GET", requestParams);

            var orders = new List<Order>();
            orders.AddRange(ordersResponse.orders);

            return orders;
        }

        /// <summary>
        ///     Retrieves orderbook data for the instrument and period provided
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="instrument">the instrument for which to get data (limited subset, mostly majors, see full docs)</param>
        /// <param name="period">
        ///     the period, in seconds, for which to get data (1 hour (3600) to 1 year (31536000), will be adjusted to nearest
        ///     valid value, see full docs)
        ///     Note that the longer the period requested, the more spaced out the snapshots will be
        /// </param>
        /// <returns>string response, no parsing is performed</returns>
        public async Task<string> GetOrderbookData(string instrument, int period)
        {
            var requestString = this.labsUrl + "" + "orderbook_data?instrument=" + instrument + "&period=" + period;

            var request = WebRequest.CreateHttp(requestString);
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.Method = "GET";

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    var stream = GetResponseStream(response);
                    var reader = new StreamReader(stream);
                    return await reader.ReadToEndAsync();
                }
            }
            catch (WebException ex)
            {
                var stream = GetResponseStream(ex.Response);
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                throw new Exception(result);
            }
        }

        /// <summary>
        ///     Retrieves the current position for the given instrument and account
        ///     This will cause an error if there is no position for that instrument
        /// </summary>
        /// <param name="accountId">the account for which to get the position</param>
        /// <param name="instrument">the instrument for which to get the position</param>
        /// <returns>Position object with the details of the position</returns>
        public async Task<Position> GetPositionAsync(int accountId, string instrument)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/positions/" + instrument;

            return await this.MakeRequestAsync<Position>(requestString);
        }

        /// <summary>
        ///     Retrieves the current non-zero positions for a given account
        /// </summary>
        /// <param name="accountId">positions will be retrieved for this account id</param>
        /// <returns>List of Position objects with the details for each position (or empty list iff no positions)</returns>
        public async Task<List<Position>> GetPositionsAsync(int accountId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/positions";

            var positionResponse = await this.MakeRequestAsync<PositionsResponse>(requestString);
            var positions = new List<Position>();
            positions.AddRange(positionResponse.positions);

            return positions;
        }

        /// <summary>
        ///     Retrieves the current rate for each of a list of instruments
        /// </summary>
        /// <param name="instruments">the list of instruments to check</param>
        /// <param name="since"></param>
        /// <returns>List of Price objects with the current price for each instrument</returns>
        public async Task<List<Price>> GetRatesAsync(List<Instrument> instruments, string since = null)
        {
            var requestBuilder = new StringBuilder(this.ratesUrl + "prices?instruments=");

            foreach (var instrument in instruments)
            {
                requestBuilder.Append(instrument.instrument + ",");
            }
            var requestString = requestBuilder.ToString().Trim(',');
            requestString = requestString.Replace(",", "%2C");

            if (!string.IsNullOrEmpty(since))
            {
                requestString += "&since=" + since;
            }

            var pricesResponse = await this.MakeRequestAsync<PricesResponse>(requestString);
            var prices = new List<Price>();
            prices.AddRange(pricesResponse.prices);

            return prices;
        }

        /// <summary>
        ///     Retrieves historical spread information data for the instrument and period provided
        ///     Note: This is not available on sandbox
        /// </summary>
        /// <param name="instrument">the instrument for which to get data (all tradable instruments supported)</param>
        /// <param name="period">
        ///     the period, in seconds, for which to get data (1 hour (3600) to 1 year (31536000), will be
        ///     adjusted to nearest valid value, see full docs)
        /// </param>
        /// <param name="unique">if false, bandwidth usage will be higher and results will include fully duplicate adjacent entries</param>
        /// <returns>List of CalendarEvent objects</returns>
        public async Task<List<SpreadPeriod>> GetSpreadData(string instrument, int period, bool unique = true)
        {
            var uniqueParam = unique ? 1 : 0;
            var requestString = this.labsUrl + "" + "spreads?instrument=" + instrument + "&period=" + period + "&unique=" + uniqueParam;

            var response = await this.MakeRequestAsync<SpreadsResponse>(requestString);

            return response.GetData();
        }

        /// <summary>
        ///     Retrieves the details for a given trade
        /// </summary>
        /// <param name="accountId">the account to which the trade belongs</param>
        /// <param name="tradeId">the ID of the trade to get the details</param>
        /// <returns>TradeData object containing the details of the trade</returns>
        public async Task<TradeData> GetTradeDetailsAsync(int accountId, long tradeId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/trades/" + tradeId;

            var trade = await this.MakeRequestAsync<TradeData>(requestString);

            return trade;
        }

        /// <summary>
        ///     Retrieves the list of open trades belonging to the account
        /// </summary>
        /// <param name="account">the account to retrieve the list for</param>
        /// <param name="requestParams">optional additional parameters for the request (name, value pairs)</param>
        /// <returns>List of TradeData objects (or empty list, if no trades)</returns>
        public async Task<List<TradeData>> GetTradeListAsync(int account, Dictionary<string, string> requestParams = null)
        {
            var requestString = this.accountUrl + "accounts/" + account + "/trades";
            var tradeResponse = await this.MakeRequestAsync<TradesResponse>(requestString, "GET", requestParams);

            var trades = new List<TradeData>();
            trades.AddRange(tradeResponse.trades);

            return trades;
        }

        /// <summary>
        ///     Retrieves the details for a given transaction
        /// </summary>
        /// <param name="accountId">the id of the account to which the transaction belongs</param>
        /// <param name="transId">the id of the transaction to retrieve</param>
        /// <returns>Transaction object with the details of the transaction</returns>
        public async Task<Transaction> GetTransactionDetailsAsync(int accountId, long transId)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/transactions/" + transId;

            var transaction = await this.MakeRequestAsync<Transaction>(requestString);

            return transaction;
        }

        /// <summary>
        ///     retrieves a list of transactions in descending order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<Transaction>> GetTransactionListAsync(int accountId, Dictionary<string, string> parameters = null)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/transactions";

            var transactions = new List<Transaction>();
            var dataResponse = await this.MakeRequestAsync<TransactionsResponse>(requestString, "GET", parameters);
            transactions.AddRange(dataResponse.transactions);

            return transactions;
        }

        /// <summary>
        ///     Used primarily for test purposes, this sends a request with the expectation of getting an error
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>null if the request succeeds.  response body as string if the request fails</returns>
        public async Task<string> MakeErrorRequest(Request request)
        {
            var requestString = this.ratesUrl + request.GetRequestString();
            return await MakeErrorRequest(requestString);
        }

        /// <summary>
        ///     Modify the specified order, updating it with the parameters provided
        /// </summary>
        /// <param name="accountId">the account the owns the order</param>
        /// <param name="orderId">the order to update</param>
        /// <param name="requestParams">the parameters to update (name, value pairs)</param>
        /// <returns>Order object containing the new details of the order (post update)</returns>
        public async Task<Order> PatchOrderAsync(int accountId, long orderId, Dictionary<string, string> requestParams)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/orders/" + orderId;
            return await this.MakeRequestWithBody<Order>("PATCH", requestParams, requestString);
        }

        /// <summary>
        ///     Modify the specified trade, updating it with the parameters provided
        /// </summary>
        /// <param name="accountId">the account that owns the trade</param>
        /// <param name="tradeId">the id of the trade to update</param>
        /// <param name="requestParams">the parameters to update (name, value pairs)</param>
        /// <returns>TradeData for the trade post update</returns>
        public async Task<TradeData> PatchTradeAsync(int accountId, long tradeId, Dictionary<string, string> requestParams)
        {
            var requestString = this.accountUrl + "accounts/" + accountId + "/trades/" + tradeId;
            return await this.MakeRequestWithBody<TradeData>("PATCH", requestParams, requestString);
        }

        /// <summary>
        ///     Posts an order on the given account with the given parameters
        /// </summary>
        /// <param name="account">the account to post on</param>
        /// <param name="requestParams">the parameters to use in the request</param>
        /// <returns>PostOrderResponse with details of the results (throws if if fails)</returns>
        public async Task<PostOrderResponse> PostOrderAsync(int account, Dictionary<string, string> requestParams)
        {
            var requestString = this.accountUrl + "accounts/" + account + "/orders";
            return await this.MakeRequestWithBody<PostOrderResponse>("POST", requestParams, requestString);
        }

        /// <summary>
        ///     Initializes a streaming events session which will stream events for the given accounts
        /// </summary>
        /// <param name="accountId">the account IDs you want to stream on</param>
        /// <returns>the WebResponse object that can be used to retrieve the events as they stream</returns>
        public async Task<WebResponse> StartEventsSession(List<int> accountId = null)
        {
            var requestString = this.streamingEventsUrl + "events";
            if (accountId != null && accountId.Count > 0)
            {
                var accountIds = string.Join(",", accountId);

                requestString += "?accountIds=" + WebUtility.UrlEncode(accountIds);
            }

            var request = WebRequest.CreateHttp(requestString);
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;

            try
            {
                var response = await request.GetResponseAsync();
                return response;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var stream = new StreamReader(response.GetResponseStream());
                var result = stream.ReadToEnd();
                throw new Exception(result);
            }
        }

        /// <summary>
        ///     Initializes a streaming rates session with the given instruments on the given account
        /// </summary>
        /// <param name="instruments">list of instruments to stream rates for</param>
        /// <param name="accountId">the account ID you want to stream on</param>
        /// <returns>the WebResponse object that can be used to retrieve the rates as they stream</returns>
        public async Task<WebResponse> StartRatesSession(List<Instrument> instruments, int accountId)
        {
            var instrumentList = string.Join(",", instruments.Select(x=> x.instrument));

            instrumentList = Uri.EscapeDataString(instrumentList);

            var requestString = this.streamingRatesUrl + "prices?accountId=" + accountId + "&instruments="
                                + instrumentList;      

            var request = WebRequest.CreateHttp(requestString);
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;

            try
            {
                var response = await request.GetResponseAsync();
                return response;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var stream = new StreamReader(response.GetResponseStream());
                var result = stream.ReadToEnd();
                throw new Exception(result);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Helper function to create the parameter string out of a dictionary of parameters
        /// </summary>
        /// <param name="requestParams">the parameters to convert</param>
        /// <returns>string containing all the parameters for use in requests</returns>
        private static string CreateParamString(Dictionary<string, string> requestParams)
        {
            var requestBody = string.Join("&", requestParams.Select(pair => WebUtility.UrlEncode(pair.Key) + "=" + WebUtility.UrlEncode(pair.Value)));

            return requestBody;
        }

        private static Stream GetResponseStream(WebResponse response)
        {
            var stream = response.GetResponseStream();
            if (response.Headers["Content-Encoding"] == "gzip")
            {
                // if we received a gzipped response, handle that
                stream = new GZipStream(stream, CompressionMode.Decompress);
            }
            return stream;
        }

        /// <summary>
        ///     Used for tests, this request avoids exceptions for normal errors, ignores successful responses and just returns
        ///     error strings
        /// </summary>
        /// <param name="requestString">the request to make</param>
        /// <returns>null if request is successful, the error response if it fails</returns>
        private async Task<string> MakeErrorRequest(string requestString)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestString);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken);

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // request succeeded -- return null
                        return null;
                    }
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var stream = new StreamReader(response.GetResponseStream());
                var result = stream.ReadToEnd();
                return result;
            }
        }

        /// <summary>
        ///     Primary (internal) request handler
        /// </summary>
        /// <typeparam name="T">The response type</typeparam>
        /// <param name="requestString">the request to make</param>
        /// <param name="method">method for the request (defaults to GET)</param>
        /// <param name="requestParams">
        ///     optional parameters (note that if provided, it's assumed the requestString doesn't contain
        ///     any)
        /// </param>
        /// <returns>response via type T</returns>
        private async Task<T> MakeRequestAsync<T>(string requestString, string method = "GET", Dictionary<string, string> requestParams = null)
        {
            if (requestParams != null && requestParams.Count > 0)
            {
                var parameters = CreateParamString(requestParams);
                requestString = requestString + "?" + parameters;
            }
            var request = WebRequest.CreateHttp(requestString);
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.Method = method;

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    var stream = GetResponseStream(response);
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (WebException ex)
            {
                var stream = GetResponseStream(ex.Response);
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                throw new Exception(result);
            }
        }

        /// <summary>
        ///     Secondary (internal) request handler. differs from primary in that parameters are placed in the body instead of the
        ///     request string
        /// </summary>
        /// <typeparam name="T">response type</typeparam>
        /// <param name="method">method to use (usually POST or PATCH)</param>
        /// <param name="requestParams">the parameters to pass in the request body</param>
        /// <param name="requestString">the request to make</param>
        /// <returns>response, via type T</returns>
        private async Task<T> MakeRequestWithBody<T>(string method, Dictionary<string, string> requestParams, string requestString)
        {
            // Create the body
            var requestBody = CreateParamString(requestParams);
            var request = WebRequest.CreateHttp(requestString);
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + this.accessToken;
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                // Write the body
                await writer.WriteAsync(requestBody);
            }

            // Handle the response
            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(response.GetResponseStream());
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                var stream = new StreamReader(response.GetResponseStream());
                var result = stream.ReadToEnd();
                throw new Exception(result);
            }
        }

        #endregion

        public bool IsSandbox()
        {
            return accountUrl.Contains("sandbox");
        }
    }
}