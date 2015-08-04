namespace TechnicalIndicatorsTests
{
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class AdxTest
    {
        #region Public Methods and Operators

        [Test]
        public void Add_WhenFirstCandleAdded_ReturnRightValues()
        {
            var adx = new Adx(14);
            var candle001 = new Candle(273, 274m, 272m, 272.75m);

            adx.Add(candle001);

            adx.TrueRanges.Should().BeEmpty();
        }

        [Test]
        public void Add_WhenSeveralCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m));

            adx.TrueRanges.Last().Should().Be(2.5m);
            adx.UpDms.Last().Should().Be(0);
            adx.DownDms.Last().Should().Be(1m);
        }

        [Test]
        public void Add_WhenSeveralCandlesAdded_ReturnRightAverageTrueRange()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m));


            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m));


            adx.TrueRanges.Last().Should().Be(5.25m);
            adx.UpDms.Last().Should().Be(0m);
            adx.DownDms.Last().Should().Be(2.75m);
        }

        [Test]
        public void Add_WhenTwoCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);
            var candle001 = new Candle(273, 274m, 272m, 272.75m);
            var candle002 = new Candle(272.75m, 273.25m, 270.25m, 270.75m);

            adx.Add(candle001);
            adx.Add(candle002);

            adx.TrueRanges.Last().Should().Be(3m);
            adx.UpDms.Last().Should().Be(0);
        }

        #endregion
    }
}