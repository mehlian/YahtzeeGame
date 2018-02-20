using NUnit.Framework;
using System;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
    public class ChiSquaredTests
    {
        [Test]
        public void ChiSquared_CanBeCreated()
        {
            ChiSquared chiSquared = new ChiSquared();
        }

        [Test]
        public void IsPassingChiSquaredTest_GivenObservedSetExpectedSetAndAlfa_ReturnsTrue()
        {
            ChiSquared chi = new ChiSquared();
            //                       #1, #2, #3, #4, #5, #6
            double[] observedSet = { 43, 39, 42, 37, 41, 44 };
            double[] expectedSet = { 41, 41, 41, 41, 41, 41 };
            double alfa = 0.01;

            var result = chi.IsPassingChiSquaredTest(observedSet, expectedSet, alfa);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsPassingChiSquaredTest_GivenObservedSetExpectedSetAndAlfa_ReturnsFalse()
        {
            ChiSquared chi = new ChiSquared();
            //                       #1, #2, #3, #4, #5, #6
            double[] observedSet = { 22, 24, 38, 30, 46, 44 };
            double[] expectedSet = { 34, 34, 34, 34, 34, 34 };
            double alfa = 0.01;

            var result = chi.IsPassingChiSquaredTest(observedSet, expectedSet, alfa);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsPassingChiSquaredTest_WhenObservedAndExpectedSetsAreSame_Throws()
        {
            ChiSquared chi = new ChiSquared();
            //                      #1,#2,#3,#4,#5,#6
            double[] observedSet = { 1, 1, 1, 1, 1, 1 };
            double[] expectedSet = { 1, 1, 1, 1, 1, 1 };
            double alfa = 0.01;

            TestDelegate result = () => chi.IsPassingChiSquaredTest(observedSet, expectedSet, alfa);

            Assert.Throws<ArgumentException>(result);
        }
    }
}
