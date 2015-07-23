namespace Console
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Timers;

    using Newtonsoft.Json;

    using TechnicalIndicators;

    public class Program
    {
        private static void Main()
        {
            var tick = new Timer(5000);
            tick.Elapsed += OnTick;
            tick.Enabled = true;

            Console.ReadKey();
        }

        private static void OnTick(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            if (timer == null)
            {
                return;
            }

            timer.Enabled = false;

            var request =
                (HttpWebRequest)
                    WebRequest.Create(
                        "http://api-sandbox.oanda.com/v1/candles?instrument=EUR_USD&count=73&candleFormat=bidask&granularity=M1&dailyAlignment=0&alignmentTimezone=America%2FNew_York");
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("Accept-Encoding: gzip, deflate");

            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                using (var reader = new StreamReader(dataStream))
                {
                    var line = reader.ReadToEnd();
                    var instrument = JsonConvert.DeserializeObject<InstrumentHistory>(line);
                    var candles_72 = instrument.Candles.Where(x => x.Complete)
                        .Take(72)
                        .OrderByDescending(x => x.Time)
                        .ToList();
                    const decimal Typical = 3;
                    var sm2_typical_72 = candles_72.Select(x => (x.CloseAsk + x.HighAsk + x.LowAsk) / Typical)
                        .Average();
                    var sm2_high_72 = candles_72.Select(x => x.HighAsk)
                        .Average();
                    var sm2_low_72 = candles_72.Select(x => x.LowAsk)
                        .Average();

                    var candles_12 = candles_72.Take(12);
                    var sm2_typical_12 = candles_12.Select(x => (x.CloseAsk + x.HighAsk + x.LowAsk) / Typical)
                        .Average();
                    var sm2_high_12 = candles_12.Select(x => x.HighAsk)
                        .Average();
                    var sm2_low_12 = candles_12.Select(x => x.LowAsk)
                        .Average();

                    var adx = candles_72.Adx(14)
                        .FirstOrDefault();
                    Debug.WriteLine(line);
                }
            }
            response.Close();

            timer.Enabled = true;
        }
    }
}