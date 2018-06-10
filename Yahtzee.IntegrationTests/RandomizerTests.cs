using NUnit.Framework;
using RandomNumberGenerator;
using System.Diagnostics;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void GetRandomNumber_ChiSquaredTestWith12000Tries_ReturnsTrue()
        {
            var randomizer = new Randomizer();
            var chiSquared = new ChiSquared();
            var observedResults = new double[6];
            var expectedResults = new double[] { 2000, 2000, 2000, 2000, 2000, 2000 };
            var alfa = 0.01; // very restricted

            for (int i = 0; i < 12000; i++)
            {
                var indexToIncrement = randomizer.GetRandomNumber(1, 6) - 1;
                observedResults[indexToIncrement]++;
            }

            var result = chiSquared.IsPassingChiSquaredTest(observedResults, expectedResults, alfa);

            Assert.IsTrue(result);
        }

        //[Test]
        //public void ChiSquared_GivenProblematicArray_ReturnsTrue()
        //{
        //    var randomizer = new Randomizer();
        //    var chiSquared = new ChiSquared();
        //    var observedResults = new double[] { 1022, 1097, 987, 1008, 931, 955 };
        //    var expectedResults = new double[] { 1000, 1000, 1000, 1000, 1000, 1000 };
        //    var alfa = 0.01; // very restricted

        //    var result = chiSquared.IsPassingChiSquaredTest(observedResults, expectedResults, alfa);

        //    Assert.IsTrue(result, $"observedResults: {observedResults}; expectedResults: {expectedResults}; alfa: {alfa}");
        //}
    }
}
