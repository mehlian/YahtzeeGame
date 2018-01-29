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
        private IRandomizer randomizer;
        private Game game;

        [SetUp]
        public void SetUp()
        {
            randomizer = Substitute.For<IRandomizer>();
            game = new Game(randomizer);
        }

        [Test]
        public void AddPlayer_PlayerNameAsString_PlayerIsAdded()
        {
            game.AddPlayer("A");
            var result = game.Players;
            var expected = new List<string> { "A" };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PlayerNameLength_DefaultPlayerNameLength_Returns10()
        {
            Assert.AreEqual(10, game.PlayerNameLength);
        }

        [Test]
        public void AddPlayer_PlayerNameIsLongerThanAllowed_ThrowsException()
        {
            game.PlayerNameLength = 10;
            TestDelegate result = () => game.AddPlayer("01234567890");

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void AddPlayer_5thPlayerIsAdded_ThrowsException()
        {
            game.AddPlayer("1");
            game.AddPlayer("2");
            game.AddPlayer("3");
            game.AddPlayer("4");
            TestDelegate result = () => game.AddPlayer("5");

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void Roll_GivenPlayersNumber_ReturnsInteger()
        {
            randomizer.Roll(1, 6).Returns(1);

            int result = game.Roll();
            int expectedFakeRollResult = 1;

            Assert.AreEqual(expectedFakeRollResult, result);
        }

    }
}
