
namespace TechnicalIndicatorsTests
{
    using FluentAssertions;

    using NUnit.Framework;

    using TechnicalIndicators;
    using System;

    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void RoundDown_Wit15Minutes_Succeeds()
        {
            var target = new DateTime(2016, 11, 1, 1, 23, 45);
            var result = target.RoundDown(TimeSpan.FromMinutes(15));

            (new DateTime(2016, 11, 1, 1, 15, 0) - result).TotalSeconds.Should().Be(0);
        }


        [Test]
        public void RoundDown_Wit45Minutes_Succeeds()
        {
            var target = new DateTime(2016, 11, 1, 1, 33, 45);
            var result = target.RoundDown(TimeSpan.FromMinutes(45));

            (new DateTime(2016, 11, 1, 1,30, 0) - result).TotalMinutes.Should().Be(0);
        }
    }
}
