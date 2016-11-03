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
            var webjobNames = new List<string> { "CobraEURUSD15MI", "CobraUSDJPY15MI", "CobraGBPUSD15MI", "CobraUSDCHF15MI" };
            foreach (var webJobName in webjobNames)
            {
                var webjobUrl = $"https://{WebsiteName}.scm.azurewebsites.net/api/continuouswebjobs/{webJobName}/start";

                var client = new HttpClient();
                var auth = "Basic "
                           + Convert.ToBase64String(
                               Encoding.UTF8.GetBytes("$forextradinghost" + ':' + "Jngj8QxdSJKnJNB2fYWqcxf2vgZJXnPsp0dy7cbTRle22edQ2lruiY7gSPoj"));
                client.DefaultRequestHeaders.Add("authorization", auth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent queryString = new StringContent(string.Empty);
                var data = client.PostAsync(webjobUrl, queryString).Result;
                var result = data.ReasonPhrase;
            }
        }

        #endregion
    }
}