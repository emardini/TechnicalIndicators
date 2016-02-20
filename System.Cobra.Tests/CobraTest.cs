namespace System.Cobra.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class CobraTest
    {
        #region Public Methods and Operators

        [Test]
        public void CanGoLong_IfBannedDay_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            var slowSmaHigh = new Sma(72);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 8)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCloseDoesNotIncreaseRelativeToTheTwoPreviousCandles_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(40, 50, 10, 10), new Candle(40, 50, 10, 10), new Candle(40, 50, 10, 45) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 14);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleIsNotUp_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(10, 50, 5, 5) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleIsNull_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleIsReversal_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>
            {
                new Candle(1.47455M, 1.47657m, 1.473517m, 1.47607m),
                new Candle(1.47607M, 1.47699m, 1.473517m, 1.47629m)
            };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(1.475m);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(1.474m);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 1.4761m, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleOpenIsLessThanFastEmaOnHigh_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(10, 50, 10, 40) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfRateLessThanfastEmaHigh_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 3, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfSlowSmaOnHighGreaterThanFastEmaOnHigh_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.CanGoLong(new Rate { Ask = 3, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfCurrentCandleCloseHigherThanPreviousCandleClose_ReturnsTrue()
        {
            Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15), new Candle(10, 20, 5, 16)).Should().BeTrue();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfCurrentCandleIsNull_ReturnsFalse()
        {
            Cobra.ConfirmPreviousCandleForAsk(new Candle(), null).Should().BeFalse();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfPreviousCandleCloseIsEqualThanCurrentCandleClose_ReturnsFalse()
        {
            Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15), new Candle(10, 20, 5, 15)).Should().BeFalse();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfPreviousCandleCloseIsHigherThanCurrentCandleClose_ReturnsFalse()
        {
            Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15), new Candle(10, 20, 5, 14)).Should().BeFalse();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfPreviousCandleIsNotUp_ReturnsFalse()
        {
            Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 8), new Candle()).Should().BeFalse();
        }

        [Test]
        public void ConfirmPreviousCandleForAsk_IfPreviousCandleIsNull_ReturnsFalse()
        {
            Cobra.ConfirmPreviousCandleForAsk(null, new Candle()).Should().BeFalse();
        }

        [Test]
        public void IsBannedDay_IfItIsBannedDay_ReturnsTrue()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            var slowSmaHigh = new Sma(72);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 8)); //Friday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.IsBannedDay().Should().BeTrue();
        }

        [Test]
        public void IsBannedDay_IfItIsNotBannedDay_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>();
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            var slowSmaHigh = new Sma(72);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentUtcDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, initialCandles, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15);

            target.IsBannedDay().Should().BeFalse();
        }

        #endregion
    }
}