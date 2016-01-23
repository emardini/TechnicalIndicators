namespace TechnicalIndicatorsTests
{
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;

    [TestFixture]
    public class SmaTest
    {
        #region Public Methods and Operators

        [Test]
        public void AddRange_OnlyStoresNbOfElementsDefinedInmaxBuffer()
        {
            var sma = new Sma(2);
            var elements = Enumerable.Range(1, 10000).Select(x => (decimal)x);
            sma.AddRange(elements);

            sma.Values.Count().Should().Be(5000);
        }

        [Test]
        public void AddRange_OnlyTakesElementsInThePeriodForCalculation()
        {
            var sma = new Sma(2);
            sma.AddRange(new[] { 0, 10m, 10m });

            sma.Values.LastOrDefault().Should().Be(10m);
            sma.Values.FirstOrDefault().Should().Be(0);
        }

        [Test]
        public void AddRange_WhenTwoValueAddedWithDifferentValue_LastSmaValueIsTheAverage()
        {
            var sma = new Sma(10);
            sma.AddRange(new[] { 0, 10m });

            sma.Values.LastOrDefault().Should().Be(5m);
            sma.Values.FirstOrDefault().Should().Be(0);
        }

        [Test]
        public void AddRange_WhenTwoValueAddedWithTheSameValue_LastSmaValueIsTheSameAsOriginalValue()
        {
            var sma = new Sma(10);
            sma.AddRange(new[] { 10m, 10m });

            sma.Values.LastOrDefault().ShouldBeEquivalentTo(10m);
        }

        [Test]
        public void Add_WhenOneValueAdded_LastSmaValueIsTheSameAsOriginalValue()
        {
            var sma = new Sma(10);
            sma.Add(10m);

            sma.Values.LastOrDefault().ShouldBeEquivalentTo(10m);
        }

        [Test]
        public void Add_WhenTwoValueAddedWithDifferentValue_LastSmaValueIsTheAverage()
        {
            var sma = new Sma(10);
            sma.Add(0m);
            sma.Add(10m);

            sma.Values.LastOrDefault().ShouldBeEquivalentTo(5m);
            sma.Values.FirstOrDefault().ShouldBeEquivalentTo(0);
        }

        [Test]
        public void Add_WhenTwoValueAddedWithTheSameValue_LastSmaValueIsTheSameAsOriginalValue()
        {
            var sma = new Sma(10);
            sma.Add(10m);
            sma.Add(10m);

            sma.Values.LastOrDefault().ShouldBeEquivalentTo(10m);
        }

        #endregion
    }
}