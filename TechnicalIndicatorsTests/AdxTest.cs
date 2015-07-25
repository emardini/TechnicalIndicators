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

            adx.TrueRanges.First().Should().Be(274);
        }
    }
}
