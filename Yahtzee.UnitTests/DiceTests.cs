using NUnit.Framework;
using Yahtzee.Core;
using NSubstitute;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class DiceTests
    {
        public Dice Make_Dice()
        {
            IRandomizer fakeRandomizer = Substitute.For<IRandomizer>();
            return new Dice(fakeRandomizer);
        }

        [Test]
        public void Dice_CanBeInitialized()
        {
            Dice dice = Make_Dice();
        }

        [Test]
        public void Status_InitialStatus_IsUnLocked()
        {
            Dice dice = Make_Dice();

            Assert.AreEqual(DiceStatus.UnLocked, dice.Status);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Roll_RollDiceToGetRandomNumber_ReturnsInteger(int expected)
        {
            IRandomizer fakeRandomizer = Substitute.For<IRandomizer>();
            Dice dice = new Dice(fakeRandomizer);
            fakeRandomizer.GetRandomInt().Returns(expected);

            int result = dice.Roll();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Result_StoreDiceRollResult_ReturnsPreviousRollResult()
        {
            IRandomizer fakeRandomizer = Substitute.For<IRandomizer>();
            Dice dice = new Dice(fakeRandomizer);
            fakeRandomizer.GetRandomInt().Returns(1);

            dice.Roll();
            int result = dice.Result;

            Assert.AreEqual(1, result);
        }

    }
}
