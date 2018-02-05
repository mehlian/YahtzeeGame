﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Yahtzee.Core;
using NSubstitute;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        private Game _game;
        private IRandomizer _randomizer;

        [SetUp]
        public void Setup()
        {
            _randomizer = Substitute.For<IRandomizer>();
            _game = new Game(_randomizer);
        }

        [Test]
        public void Game_CanBeCreated()
        {
            Game game = new Game(_randomizer);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void RollDice_GivenListOfDicesThatAreUnlocked_ReturnsListOfDicesWithNewResults(int expected)
        {
            _randomizer.Roll(1, 6).Returns(expected);
            List<Dice> dices = new List<Dice>
            {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            List<Dice> result = _game.RollDice(dices);

            Assert.That(result.Any(d => d.Result == expected));
        }

        [Test]
        public void RollDice_GivenListOfDicesThatAreLocked_ReturnsListOfDicesWithOldResults()
        {
            _randomizer.Roll(1, 6).Returns(1);
            List<Dice> dices = new List<Dice>
            {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            _game.RollDice(dices);
            dices.ForEach(d => d.Lock());
            _randomizer.Roll(1, 6).Returns(2);
            List<Dice> result = _game.RollDice(dices);

            Assert.IsTrue(result.Any(d => d.Result == 1));
        }

    }
}
