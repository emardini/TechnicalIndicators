namespace KeepAlive
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Microsoft.Azure.WebJobs;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.

        #region Public Methods and Operators

        public static void KeepAlive([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo timer)
        {
            const string WebsiteName = "forextradinghost";
            var webjobNames = new List<string> { "CobraEURUSD", "CobraGBPUSD", "CobraUSDJPY" };
            foreach (var webJobName in webjobNames)
            {
                var webjobUrl = string.Format("https://{0}.scm.azurewebsites.net/api/continuouswebjobs/{1}", WebsiteName, webJobName);

                var client = new HttpClient();
                var auth = "Basic "
                           + Convert.ToBase64String(
                               Encoding.UTF8.GetBytes("$ForexTradingHost" + ':' + "Pi0EqW2cgFuk8bEadpn2EfEsv9ap5Bc8F67LuqpuprvjHL28A5baaZvfzayX"));
                client.DefaultRequestHeaders.Add("authorization", auth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = client.GetStringAsync(webjobUrl).Result;
                var result = JsonConvert.DeserializeObject(data) as JObject;
            }
        }

        #endregion
    }
}