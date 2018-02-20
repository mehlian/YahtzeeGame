using NUnit.Framework;
using RandomNumberGenerator;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void GetRandomNumber_ChiSquaredTestWith6000Tries_ReturnsTrue()
        {
            Randomizer randomizer = new Randomizer();
            ChiSquared chiSquared = new ChiSquared();
            double[] observedResults = new double[6];
            double[] expectedResults = { 1000, 1000, 1000, 1000, 1000, 1000 };
            double alfa = 0.01; // very restricted

            for (int i = 0; i < 6000; i++)
            {
                var indexToIncrement = randomizer.GetRandomNumber(1, 6) - 1;
                observedResults[indexToIncrement]++;
            }

            var result = chiSquared.IsPassingChiSquaredTest(observedResults, expectedResults, alfa);

            Assert.IsTrue(result);
        }
    }
}
