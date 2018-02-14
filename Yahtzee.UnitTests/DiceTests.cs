using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class DiceTests
    {
        private Dice dice;

        [SetUp]
        public void Setup()
        {
            dice = new Dice();
        }

        [Test]
        public void Dice_CanBeCreated()
        {
            Dice dice = new Dice();
        }

        [Test]
        public void Dice_SideNumber_Returns6()
        {
            var result = dice.SideNumber;

            Assert.AreEqual(6, result);
        }

        [Test]
        public void IsUnlocked_InitialState_ReturnsTrue()
        {
            bool result = dice.IsUnlocked;

            Assert.IsTrue(result);
        }

        [Test]
        public void Lock_ChangeDiceStateToLocked_IsUnlockedReturnsFalse()
        {
            dice.Lock();
            bool result = dice.IsUnlocked;

            Assert.AreEqual(false, result);
        }

        [Test]
        public void Unlock_ChangeDiceStateToUnlocked_IsUnlockedReturnsTrue()
        {
            dice.Lock();
            dice.Unlock();
            bool result = dice.IsUnlocked;

            Assert.AreEqual(true, result);
        }

        [Test]
        public void Result_GivenNumberAsRollResult_NumberIsSaved()
        {
            dice.Result = 1;

            Assert.AreEqual(1, dice.Result);
        }
    }
}
