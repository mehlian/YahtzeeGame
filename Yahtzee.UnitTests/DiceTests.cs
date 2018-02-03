using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void IsLocked_DefaultValue_ReturnsFalse()
        {
            Assert.AreEqual(false, dice.IsLocked);
        }

        [Test]
        public void GetResult_DefaultValue_ReturnsZero()
        {
            var result = dice.Result;

            Assert.AreEqual(0, dice.Result);
        }

        [Test]
        public void SetResult_WhenIsLocked_ValueIsNotStored()
        {
            dice.Result = 1;
            dice.IsLocked = true;
            dice.Result = 2;

            Assert.AreEqual(1, dice.Result);
        }

    }
}
