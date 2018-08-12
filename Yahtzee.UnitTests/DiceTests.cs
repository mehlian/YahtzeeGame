using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class DiceTests
    {
        private Dice _dice;

        [SetUp]
        public void Setup()
        {
            _dice = new Dice();
        }

        [TestCase]
        public void Dice_CanBeCreated()
        {
            Dice dice = new Dice();
        }

        [TestCase]
        public void SideNumber_InitialState_ReturnsDefaultValue()
        {
            var expectedDefaultValue = 6;

            var result = _dice.SideNumber;

            Assert.AreEqual(expectedDefaultValue, result);
        }

        [TestCase]
        public void IsUnlocked_InitialState_ReturnsTrue()
        {
            var result = _dice.IsUnlocked;

            Assert.IsTrue(result);
        }

        [TestCase]
        public void Lock_DiceStateChangedToLocked_IsUnlockedReturnsFalse()
        {
            _dice.Lock();

            var result = _dice.IsUnlocked;

            Assert.IsFalse(result);
        }

        [TestCase]
        public void Unlock_DiceStateChangedToUnlocked_IsUnlockedReturnsTrue()
        {
            _dice.Lock();
            _dice.Unlock();

            var result = _dice.IsUnlocked;

            Assert.IsTrue(result);
        }

        [TestCase]
        public void Result_GivenNumberAsRollResult_NumberIsStored()
        {
            _dice.Result = 1;

            var result = _dice.Result;

            Assert.AreEqual(1, result);
        }
    }
}
