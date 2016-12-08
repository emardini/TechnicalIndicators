
namespace TechnicalIndicatorsTests
{
    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    using System;

    [TestFixture]
    public class CandleBuilderTest
    {
        [Test]
        public void AddRate_Succeeds()
        {
            var target = new CandleBuilder(TimeSpan.FromMinutes(15));
            Candle result = null; 
            target.NewCandleCreated += delegate (object o, CandleBuilderEventArgs e){ result = e.NewCandle; };

            target.AddRate(new Rate() {Ask = 1, Bid = 2, Time = new DateTime(2016, 11, 1, 1, 23, 45) });
            target.AddRate(new Rate() { Ask = 2, Bid = 10, Time = new DateTime(2016, 11, 1, 1, 23, 46) });
            target.AddRate(new Rate() { Ask = 0.01m, Bid = 1, Time = new DateTime(2016, 11, 1, 1, 23, 47) });
            target.AddRate(new Rate() { Ask = 4, Bid = 5, Time = new DateTime(2016, 11, 1, 1, 23, 48) });

            target.AddRate(new Rate() { Ask = 1, Bid = 2, Time = new DateTime(2016, 11, 1, 1, 50, 45) });

            result.ShouldBeEquivalentTo(new Candle(1.5m, 6m, 1.01m/2m, 4.5m, new DateTime(2016, 11, 1, 1, 15, 0)));
        }
    }
}
