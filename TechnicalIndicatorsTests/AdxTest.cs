﻿namespace TechnicalIndicatorsTests
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class AdxTest
    {
        #region Public Methods and Operators

        [Test]
        public void Add_WhenFifteenCandlesAdded_ReturnRightAverageTrueRange()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m,DateTime.UtcNow));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m,DateTime.UtcNow));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m,DateTime.UtcNow));

            adx.TrueRangesAccum.Last().Should().BeApproximately(43.32m, 0.01m);
            adx.UpDmAccum.Last().Should().BeApproximately(8.82m, 0.01m);
            adx.DownDmAccum.Last().Should().BeApproximately(15.75m, 0.01m);
            adx.UpDiAccum.Last().Should().BeApproximately(.2036m, 0.01m);
            adx.DownDiAccum.Last().Should().BeApproximately(0.3635m, 0.01m);
            adx.Dxs.Last().Should().BeApproximately(0.28m, 0.01m);
        }

        [Test]
        public void Add_WhenFirstCandleAdded_TrueRangeListIsEmpty()
        {
            var adx = new Adx(14);
            var candle001 = new Candle(273, 274m, 272m, 272.75m, DateTime.UtcNow);

            adx.Add(candle001);

            adx.TrueRanges.Should().BeEmpty();
        }

        [Test]
        public void Add_WhenSeventeenCandlesAdded_ReturnRightDx()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m,DateTime.UtcNow));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m,DateTime.UtcNow));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 264m, 261.5m, 264m,DateTime.UtcNow));
            adx.Add(new Candle(267m, 268m, 266.25m, 266.5m,DateTime.UtcNow));

            adx.Dxs.Last().Should().BeApproximately(0.13m, 0.01m);
        }

        [Test]
        public void Add_WhenSixCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));

            adx.TrueRanges.Last().Should().Be(2.5m);
            adx.UpDms.Last().Should().Be(0);
            adx.DownDms.Last().Should().Be(1m);
        }

        [Test]
        public void Add_WhenSixteenCandlesAdded_ReturnRightAverageTrueRange()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m,DateTime.UtcNow));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m,DateTime.UtcNow));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 264m, 261.5m, 264m,DateTime.UtcNow));

            adx.TrueRangesAccum.Last().Should().BeApproximately(42.98m, 0.01m);
            adx.UpDmAccum.Last().Should().BeApproximately(8.19m, 0.01m);
            adx.DownDmAccum.Last().Should().BeApproximately(16.37m, 0.01m);
            adx.UpDiAccum.Last().Should().BeApproximately(.1905m, 0.01m);
            adx.DownDiAccum.Last().Should().BeApproximately(.38m, 0.01m);
            adx.Dxs.Last().Should().BeApproximately(0.33m, 0.01m);
        }

        [Test]
        public void Add_WhenThreeCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));

            adx.TrueRanges.Last().Should().Be(2.25m);
            adx.UpDms.Last().Should().Be(0);
            adx.DownDms.Last().Should().Be(.5m);
        }

        [Test]
        public void Add_WhenTwentyEightCandlesAdded_ReturnRightAdx()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m,DateTime.UtcNow));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m,DateTime.UtcNow));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 264m, 261.5m, 264m,DateTime.UtcNow));
            adx.Add(new Candle(267m, 268m, 266.25m, 266.5m,DateTime.UtcNow));

            adx.Add(new Candle(265m, 266m, 264.25m, 265.25m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 274m, 267m, 273m,DateTime.UtcNow));
            adx.Add(new Candle(277m, 277.5m, 273.5m, 276.75m,DateTime.UtcNow));
            adx.Add(new Candle(275m, 277m, 272.5m, 273m,DateTime.UtcNow));
            adx.Add(new Candle(271m, 272m, 269.5m, 270.25m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.75m, 264m, 266.75m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 269.25m, 263m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 266m, 263.5m, 265.5m,DateTime.UtcNow));
            adx.Add(new Candle(264m, 265m, 262m, 262.25m,DateTime.UtcNow));
            adx.Add(new Candle(262m, 264.75m, 261.5m, 262.75m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 261m, 255.5m, 255.5m,DateTime.UtcNow));

            adx.Values.Last().Should().BeApproximately(0.15m, 0.01m);
        }

        [Test]
        public void Add_WhenTwentyNineCandlesAdded_ReturnRightAdx()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));
            adx.Add(new Candle(270.75m, 272m, 269.75m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 270.75m, 268m, 269.25m,DateTime.UtcNow));
            adx.Add(new Candle(269.25m, 270m, 269m, 269.75m,DateTime.UtcNow));
            adx.Add(new Candle(269.75m, 270.5m, 268m, 270m,DateTime.UtcNow));
            adx.Add(new Candle(266.5m, 268.5m, 266.5m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 265.5m, 263m, 263.25m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 262.5m, 259m, 260.25m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 263.5m, 260m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(263m, 269.5m, 263m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.25m, 265m, 267m,DateTime.UtcNow));
            adx.Add(new Candle(265.5m, 267.5m, 265.5m, 265.75m,DateTime.UtcNow));
            adx.Add(new Candle(266m, 269.75m, 266m, 268.5m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 268.25m, 263.25m, 264.25m,DateTime.UtcNow));
            adx.Add(new Candle(263.25m, 264m, 261.5m, 264m,DateTime.UtcNow));
            adx.Add(new Candle(267m, 268m, 266.25m, 266.5m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 266m, 264.25m, 265.25m,DateTime.UtcNow));
            adx.Add(new Candle(270m, 274m, 267m, 273m,DateTime.UtcNow));
            adx.Add(new Candle(277m, 277.5m, 273.5m, 276.75m,DateTime.UtcNow));
            adx.Add(new Candle(275m, 277m, 272.5m, 273m,DateTime.UtcNow));
            adx.Add(new Candle(271m, 272m, 269.5m, 270.25m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 267.75m, 264m, 266.75m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 269.25m, 263m, 263m,DateTime.UtcNow));
            adx.Add(new Candle(265m, 266m, 263.5m, 265.5m,DateTime.UtcNow));
            adx.Add(new Candle(264m, 265m, 262m, 262.25m,DateTime.UtcNow));
            adx.Add(new Candle(262m, 264.75m, 261.5m, 262.75m,DateTime.UtcNow));
            adx.Add(new Candle(260m, 261m, 255.5m, 255.5m,DateTime.UtcNow));
            adx.Add(new Candle(255m, 257.5m, 253m, 253m,DateTime.UtcNow));

            adx.Values.Last().Should().BeApproximately(0.16m, 0.01m);
        }

        [Test]
        public void Add_WhenTwoCandlesAdded_ReturnRightValues()
        {
            var adx = new Adx(14);

            adx.Add(new Candle(273, 274m, 272m, 272.75m,DateTime.UtcNow));
            adx.Add(new Candle(272.75m, 273.25m, 270.25m, 270.75m,DateTime.UtcNow));

            adx.TrueRanges.Last().Should().Be(3m);
            adx.UpDms.Last().Should().Be(0);
            adx.DownDms.Last().Should().Be(1.75m);
        }

        #endregion
    }
}