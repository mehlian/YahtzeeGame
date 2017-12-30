using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void Randomizer_CanBeInitialized()
        {
            Randomizer randomizer = new Randomizer();
        }

        [Test]
        public void Randomizer_ImplementsIRandomizerInterface()
        {
            Randomizer randomizer = new Randomizer();

            Assert.IsInstanceOf<IRandomizer>(randomizer);
        }

        [Test]
        public void GetRandomInt_ForGivenNumbersRange_ReturnsNumberGreaterOrEqualToMinValue()
        {
            Randomizer randomizer = new Randomizer();
            int minNumber = 1;
            int maxNumber = 6;

            int result = randomizer.GetRandomInt(minNumber, maxNumber);

            Assert.GreaterOrEqual(result, minNumber);
        }

        [Test]
        public void GetRandomInt_ForGivenNumbersRange_ReturnsNumberLesserOrEqualToMaxValue()
        {
            Randomizer randomizer = new Randomizer();
            int minNumber = 1;
            int maxNumber = 6;

            int result = randomizer.GetRandomInt(minNumber, maxNumber);

            Assert.LessOrEqual(result, maxNumber);
        }

    }
}
