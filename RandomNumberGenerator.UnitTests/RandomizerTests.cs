using NUnit.Framework;
using Yahtzee.Core;

namespace RandomNumberGenerator.UnitTests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void Randomizer_CanBeCreated()
        {
            Randomizer randomizer = new Randomizer();
        }

        [Test]
        public void Randomizer_ImplementsIRandomizer()
        {
            Randomizer randomizer = new Randomizer();

            Assert.IsInstanceOf<IRandomizer>(randomizer);
        }

        [Test]
        public void GetRandomNuber_ForGivenMinValue_ReturnsValueGreaterOrEqualToMinValue()
        {
            Randomizer randomizer = new Randomizer();

            var result = randomizer.GetRandomNumber(1, 6);

            Assert.GreaterOrEqual(result, 1);
        }

        [Test]
        public void GetRandomNuber_ForGivenMaxValue_ReturnsValueLesserOrEqualToMaxValue()
        {
            Randomizer randomizer = new Randomizer();

            var result = randomizer.GetRandomNumber(1, 6);

            Assert.LessOrEqual(result, 6);
        }
    }
}
