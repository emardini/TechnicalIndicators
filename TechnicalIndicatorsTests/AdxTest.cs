using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalIndicatorsTests
{
    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class AdxTest
    {
        [Test]
        public void Add_WhenFirstCandleAdded_ReturnRightValues()
        {
            var adx = new Adx(14);
            var candle001 = new Candle(273, 272.75m, 272m, 274m);

            adx.Add(candle001);

            adx.TrueRanges.Should().BeEmpty();
        }

        [Test]
        public void Add_WhenTwoCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);
            var candle001 = new Candle(273, 272.75m, 272m, 274m);
            var candle002 = new Candle(272.75m, 270.75m, 270.25m, 273.25m);

            adx.Add(candle001);
            adx.Add(candle002);

            adx.TrueRanges.Last().Should().Be(3m);
            adx.UpDms.Last().Should().Be(0);
        }

        [Test]
        public void Add_WhenSeveralCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 272.75m, 272m, 274m));
            adx.Add(new Candle(272.75m, 270.75m, 270.25m, 273.25m));
            adx.Add(new Candle(270.75m, 270m, 269.75m, 272m));
            adx.Add(new Candle(270m, 269.25m, 268m, 270.75m));
            adx.Add(new Candle(269.25m, 269.75m, 269m, 270m));
            adx.Add(new Candle(269.75m, 270m, 268m, 270.5m));

            adx.TrueRanges.Last().Should().Be(2.5m);
            adx.UpDms.Last().Should().Be(0);
            adx.DownDms.Last().Should().Be(1m);
        }
    }
}
