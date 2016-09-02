namespace KeepAlive
{
    using System.Diagnostics;

    using Microsoft.Azure.WebJobs;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage

        #region Methods

        private static void Main()
        {
            var config = new JobHostConfiguration();
            config.Tracing.ConsoleLevel = TraceLevel.Verbose;
            config.UseTimers();

            var host = new JobHost(config);
            host.RunAndBlock();
        }

        #endregion
    }
}