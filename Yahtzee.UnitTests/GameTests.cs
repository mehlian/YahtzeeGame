using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        private IRandomizer _randomizer;
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _randomizer = Substitute.For<IRandomizer>();
            _game = new Game(_randomizer);
        }

        [Test]
        public void AddPlayer_PlayerNameAsString_PlayerIsAdded()
        {
            _game.AddPlayer("A");
            var result = _game.Players;
            var expected = new List<string> { "A" };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PlayerNameLength_DefaultValue_Returns10()
        {
            Assert.AreEqual(10, _game.PlayerNameLength);
        }

        [Test]
        public void AddPlayer_PlayerNameIsLongerThanAllowed_ThrowsException()
        {
            _game.PlayerNameLength = 10;
            TestDelegate result = () => _game.AddPlayer("01234567890");

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void AddPlayer_5thPlayerIsAdded_ThrowsException()
        {
            _game.AddPlayer("1");
            _game.AddPlayer("2");
            _game.AddPlayer("3");
            _game.AddPlayer("4");
            TestDelegate result = () => _game.AddPlayer("5");

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void AddPlayer_SamePlayerIsAddedTwice_Throws()
        {
            _game.AddPlayer("1");

            TestDelegate result = () => _game.AddPlayer("1");

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void RollResult_GivenDieNumber_ReturnsDieResult()
        {
            var result = _game.RollResult(1);

            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(6)]
        public void RollResult_GivenDieNumberIsOutOfRange1to5_Throws(int dieNumber)
        {
            TestDelegate result = () => _game.RollResult(dieNumber);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void DieStatus_GivenDieNumber_ReturnsDieLockStatus(
            [Range(1, 5)]int dieNumber)
        {
            bool result = _game.DieStatus(dieNumber);

            Assert.AreEqual(false, result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(6)]
        public void DieStatus_GivenDieNumberIsOutOfRange1to5_Throws(int dieNumber)
        {
            TestDelegate result = () => _game.DieStatus(dieNumber);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void LockDie_GivenDieNumber_ChangeIsLockedToTrue(
            [Range(1, 5)]int dieNumber)
        {
            _game.LockDie(dieNumber);
            bool result = _game.DieStatus(dieNumber);

            Assert.AreEqual(true, result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(6)]
        public void LockDie_GivenDieNumberIsOutOfRange1to5_Throws(int dieNumber)
        {
            TestDelegate result = () => _game.LockDie(dieNumber);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void Roll_InitialRoll_ReturnsRollResult(
            [Range(1, 5)]int dieNumber)
        {
            _randomizer.Roll(1, 6).Returns(1);
            _game.Roll();

            int expectedFakeRollResult = 1;
            int result = _game.RollResult(dieNumber);
            Assert.AreEqual(expectedFakeRollResult, result);
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(5, 1)]
        public void Roll_OneDieIsLocked_LockedDieResultDoesntChange(
            int dieNumber, int expected)
        {
            _randomizer.Roll(1, 6).Returns(1);
            _game.LockDie(1);
            _game.Roll();

            int expectedFakeRollResult = expected;
            int result = _game.RollResult(dieNumber);
            Assert.AreEqual(expectedFakeRollResult, result);
        }

        [Test]
        public void CheckOptions_GivenPlayerName_ReturnListOfAvailableCategories()
        {
            _randomizer.Roll(1, 6).Returns(1);

            _game.AddPlayer("1");
            _game.Roll();
            _game.CheckOptions("1");
        }
    }
}
