namespace WebJobHost.Controllers
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web.Http;

    [RoutePrefix("jobs")]
    public class JobsController : ApiController
    {
        #region Public Methods and Operators

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var webJobNamesCsv = ConfigurationManager.AppSettings["WebJobNamesCsv"];
            var webJobCredentials = ConfigurationManager.AppSettings["WebJobCredentials"];
            var webJobHostUrl = ConfigurationManager.AppSettings["WebJobHostUrl"];

            var webjobNames = webJobNamesCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var webJobName in webjobNames)
            {
                var webjobUrl = string.Format(webJobHostUrl, webJobName);
                var client = new HttpClient();
                var auth = "Basic "
                           + Convert.ToBase64String(
                               Encoding.UTF8.GetBytes(webJobCredentials));
                client.DefaultRequestHeaders.Add("authorization", auth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent queryString = new StringContent(string.Empty);
                var data = client.PostAsync(webjobUrl, queryString).Result;
                var result = data.ReasonPhrase;
            }

            return this.Ok();
        }

        #endregion
    }
}