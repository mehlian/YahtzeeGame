using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
    public class ChiSquaredTests
    {
        [Test]
        public void ChiSquared_CanBeInitialized()
        {
            ChiSquared chi = new ChiSquared();
        }

        [Test]
        public void ChiSquared_CheckIfDiceIsFairForGivenAlfa_ReturnsTrue()
        {
            ChiSquared chi = new ChiSquared();
            //                               #1, #2, #3, #4, #5, #6
            double[] observedRollResults = { 43, 39, 42, 37, 41, 44 };
            double[] expectedRollResults = { 41, 41, 41, 41, 41, 41 };
            double alfa = 0.01;

            bool isFair = chi.CheckPValue(observedRollResults, expectedRollResults, alfa);

            Assert.IsTrue(isFair);
        }

        [Test]
        public void ChiSquared_CheckIfDiceIsFairForGivenAlfa_ReturnsFalse()
        {
            ChiSquared chi = new ChiSquared();
            //                               #1, #2, #3, #4, #5, #6
            double[] observedRollResults = { 22, 24, 38, 30, 46, 44 };
            double[] expectedRollResults = { 34, 34, 34, 34, 34, 34 };
            double alfa = 0.01;

            bool isFair = chi.CheckPValue(observedRollResults, expectedRollResults, alfa);

            Assert.IsFalse(isFair);
        }

        [Test]
        public void ChiSquared_WhenObservedAndExpectedRollResultsAreSame_ThrowsArgumentException()
        {
            ChiSquared chi = new ChiSquared();
            //                              #1,#2,#3,#4,#5,#6
            double[] observedRollResults = { 1, 1, 1, 1, 1, 1 };
            double[] expectedRollResults = { 1, 1, 1, 1, 1, 1 };
            double alfa = 0.01;

            Assert.Throws<ArgumentException>(()=> chi.CheckPValue(observedRollResults, expectedRollResults, alfa));
        }
    }
}
