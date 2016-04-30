namespace BackTestAdapterConsole
{
    using System;
    using System.Cobra;
    using System.Collections.Generic;
    using System.IO;

    using BrokerAdapter.BackTest;

    using CsvHelper;

    using TechnicalIndicators;

    public class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            var backTestAdapter = new BackTestAdapter();

            const string Instrument = "EUR_USD";

            var dataPoint = new { r = 0m, rt=DateTime.Now, h=0m, l=0m, o=0m, c=0m, ct=DateTime.Now };
            var dataPoints = ToEmptyItemList(dataPoint);
            for (var index = 0; index < 350; index++)
            {
                var rate = backTestAdapter.GetRate(Instrument);
                var candle = backTestAdapter.GetLastCandle(Instrument, 10, rate.Time);
                if(candle == null) continue;
                dataPoints.Add(new { r = rate.Ask, rt=rate.Time, h=candle.High, l=candle.Low, o=candle.Open, c=candle.Close, ct=candle.Timestamp });
                Console.WriteLine("L:{0}, H:{1}, CT:{2}, R:{3}, RT:{4}", candle.Low, candle.High, candle.Timestamp, rate.Ask, rate.Time);   
            }

            backTestAdapter.Reset();

            using (TextWriter writer = File.CreateText("output.csv"))
            {
                var csv = new CsvWriter(writer);
                csv.WriteRecords(dataPoints);
            }

            var system = new Cobra(new Adx(14),
                new List<Candle>(),
                new Ema(12),
                new Ema(12),
                new Sma(72),
                new Sma(72),
                new SimpleDateProvider(),
                Instrument,
                10,
                backTestAdapter,
                backTestAdapter,
                5027596,
                true);

            for (var i = 0; i < 330; i++)
            {
                system.CheckRate();
            }

            Console.ReadKey();
        }

        public static List<T> ToEmptyItemList<T>(T that)
        {
            var list = new List<T> { that };
            list.Clear();

            return list;
        }

        #endregion
    }
}