namespace TechnicalIndicatorsTests
{
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class EmaTest
    {
        #region Public Methods and Operators

        [Test]
        public void AddRange_OnlyStoresNbOfElementsDefinedInmaxBuffer()
        {
            var target = new Ema(2);
            var elements = Enumerable.Range(1, 10000).Select(x => (decimal)x);
            target.AddRange(elements);

            target.Values.Count().Should().Be(5000);
        }

        [Test]
        public void AddRange_OnlyTakesElementsInThePeriodForCalculation()
        {
            var target = new Ema(2);
            target.AddRange(new[] { 0, 10m, 10m });

            target.Values.LastOrDefault().Should().Be(80m / 9m);
            target.Values.FirstOrDefault().Should().Be(0);
        }

        [Test]
        //Data for test taken from http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:moving_averages&gclid=Cj0KEQjw_rytBRDVhZeQrbzn_q0BEiQAjnbSHDII7a6P7Nw3thyjccCTfRSN9Qo2MaiQmMwa0l_shMgaAjUU8P8HAQ
        public void AddRange_TestWithKnownValues_Succeed()
        {
            var target = new Ema(10);

            var elements = (new[]
            {
                22.27, 22.19, 22.08, 22.17, 22.18, 22.13, 22.23, 22.43, 22.24, 22.29, 22.15, 22.39, 22.38, 22.61, 23.36, 24.05, 23.75, 23.83, 23.95,
                23.63, 23.82, 23.87, 23.65, 23.19, 23.10, 23.33, 22.68, 23.10, 22.40, 22.17
            }).Select(x => (decimal)x);
            target.AddRange(elements);
            target.Values.LastOrDefault().Should().BeApproximately(22.915536m, 0.000001m);
        }

        [Test]
        public void AddRange_WhenTwoDifferentValueAdded_LastEmaValueIsTheAverage()
        {
            var target = new Ema(10);
            target.AddRange(new[] { 0m, 10m });

            target.Values.LastOrDefault().Should().BeApproximately((180m) / 99m, 0.000000001m);
            target.Values.FirstOrDefault().Should().Be(0);
        }

        [Test]
        public void Add_WhenOneValueAdded_LastEmaValueIsTheSameAsOriginalValue()
        {
            var target = new Ema(10);
            target.Add(10m);

            target.Values.LastOrDefault().ShouldBeEquivalentTo(10m);
        }

        [Test]
        public void Add_WhenTwoValueAddedWithDifferentValue_LastEmaValueIsTheAverage()
        {
            var target = new Ema(10);
            target.Add(0m);
            target.Add(10m);

            target.Values.LastOrDefault().Should().BeApproximately((180m) / 99m, 0.000000001m);
            target.Values.FirstOrDefault().ShouldBeEquivalentTo(0);
        }

        [Test]
        public void Add_WhenTwoValueAddedWithTheSameValue_LastEmaValueIsTheSameAsOriginalValue()
        {
            var target = new Ema(10);
            target.Add(10m);
            target.Add(10m);

            target.Values.LastOrDefault().ShouldBeEquivalentTo(10m);
        }

        #endregion
    }
}