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

        readonly Mock<ITradingAdapter> tradingAdapterMock = new Mock<ITradingAdapter>();
        readonly Mock<IRateProvider> rateProviderMock = new Mock<IRateProvider>();

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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 8)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCloseDoesNotIncreaseRelativeToTheTwoPreviousCandles_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(40, 50, 10, 10, DateTime.UtcNow.AddMinutes(-20)), new Candle(40, 50, 10, 10, DateTime.UtcNow.AddMinutes(-10)), new Candle(40, 50, 10, 45, DateTime.UtcNow) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 10, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleIsNotUp_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(10, 50, 5, 5, DateTime.UtcNow) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.CanGoLong(new Rate { Ask = 25, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleIsReversal_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle>
            {
                new Candle(1.47455M, 1.47657m, 1.473517m, 1.47607m, DateTime.UtcNow.AddMinutes(-10)),
                new Candle(1.47607M, 1.47699m, 1.473517m, 1.47629m, DateTime.UtcNow)
            };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(1.475m);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(1.474m);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 10, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.CanGoLong(new Rate { Ask = 1.4761m, Instrument = "EURUSD" }).Should().BeFalse();
        }

        [Test]
        public void CanGoLong_IfCurrentCandleOpenIsLessThanFastEmaOnHigh_ReturnsFalse()
        {
            var adx = new Adx(14);
            var initialCandles = new List<Candle> { new Candle(10, 50, 10, 40, DateTime.UtcNow) };
            var fastEmaHigh = new Ema(12);
            var fastEmaLow = new Ema(12);
            fastEmaHigh.Add(20);
            var slowSmaHigh = new Sma(72);
            slowSmaHigh.Add(10);
            var slowSmaLow = new Sma(72);
            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.CanGoLong(new Rate { Ask = 3, Instrument = "EURUSD" }).Should().BeFalse();
        }

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfCurrentCandleCloseHigherThanPreviousCandleClose_ReturnsTrue()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15, DateTime.UtcNow), new Candle(10, 20, 5, 16, DateTime.UtcNow)).Should().BeTrue();
        //}

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfCurrentCandleIsNull_ReturnsFalse()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(new Candle(), null).Should().BeFalse();
        //}

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfPreviousCandleCloseIsEqualThanCurrentCandleClose_ReturnsFalse()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15, DateTime.UtcNow), new Candle(10, 20, 5, 15, DateTime.UtcNow)).Should().BeFalse();
        //}

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfPreviousCandleCloseIsHigherThanCurrentCandleClose_ReturnsFalse()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 15, DateTime.UtcNow), new Candle(10, 20, 5, 14, DateTime.UtcNow)).Should().BeFalse();
        //}

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfPreviousCandleIsNotUp_ReturnsFalse()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(new Candle(10, 20, 5, 8, DateTime.UtcNow), new Candle()).Should().BeFalse();
        //}

        //[Test]
        //public void ConfirmPreviousCandleForAsk_IfPreviousCandleIsNull_ReturnsFalse()
        //{
        //    Cobra.ConfirmPreviousCandleForAsk(null, new Candle()).Should().BeFalse();
        //}

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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 8)); //Friday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.IsTradingDay().Should().BeTrue();
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
            dateProvider.Setup(x => x.GetCurrentEastDateTimeDate()).Returns(new DateTime(2016, 1, 7)); //Thursday
            var target = new Cobra(adx, fastEmaHigh, fastEmaLow, slowSmaHigh, slowSmaLow, dateProvider.Object, "EURUSD", 15, tradingAdapterMock.Object, rateProviderMock.Object, 0);

            target.IsTradingDay().Should().BeFalse();
        }

        #endregion
    }
}