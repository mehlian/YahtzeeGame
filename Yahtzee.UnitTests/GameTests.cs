using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Yahtzee.Core;
using NSubstitute;
using System;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        private Game _game;
        private IRandomizer _randomizer;

        private List<Dice> MakeNewDiceSet()
        {
            return new List<Dice>
            {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };
        }

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
        public void RollDice_GivenListOfDicesThatAreUnlocked_RollResultsAreSaved(int expected)
        {
            _randomizer.Roll(1, 6).Returns(expected);
            List<Dice> dices = MakeNewDiceSet();

            _game.RollDice(dices);

            Assert.That(_game.RollResult.All(d => d.Result == expected));
        }

        [Test]
        public void RollDice_GivenListOfDicesThatAreLocked_ReturnsListOfDicesWithOldResults()
        {
            _randomizer.Roll(1, 6).Returns(1);
            List<Dice> dices = MakeNewDiceSet();

            _game.RollDice(dices);
            dices.ForEach(d => d.Lock());
            _randomizer.Roll(1, 6).Returns(2);
            _game.RollDice(dices);

            Assert.IsTrue(_game.RollResult.All(d => d.Result == 1));
        }

        [Test]
        public void NewGame_PlayerNameAsString_PlayerIsAddedToTheGame()
        {
            string[] playerName = { "A" };
            _game.NewGame(playerName);

            Assert.IsTrue(_game.Players.All(x => x == "A"));
        }

        [Test]
        public void NewGame_PlayerNamesAsStringArray_PlayersAreAdded()
        {
            string[] playersName = { "A", "B", "C", "D" };
            _game.NewGame(playersName);

            Assert.IsTrue(_game.Players == playersName);
        }

        [Test]
        public void NewGame_ArrayOfPlayerNamesIsLongerThan4_Throws()
        {
            string[] playersName = { "1", "2", "3", "4", "5" };
            TestDelegate result = () => _game.NewGame(playersName);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void NewGame_PlayerNameAsString_CreatesNewColumnForThatPlayer()
        {
            string[] playerName = { "A" };
            List<Dice> dice = MakeNewDiceSet();

            _game.NewGame(playerName);
            _game.RollDice(dice);
            var result = _game.GetAvailableOptions(playerName[0]);

            Assert.IsNotNull(_game.Table[playerName[0]]);
        }
                
        [Test]
        [TestCase(1, 1, 1, 1, 1, Category.Aces, 5)]
        [TestCase(1, 1, 2, 2, 3, Category.Aces, 2)]
        [TestCase(2, 2, 2, 2, 2, Category.Twos, 10)]
        [TestCase(2, 2, 3, 3, 4, Category.Twos, 4)]
        [TestCase(3, 3, 3, 3, 3, Category.Threes, 15)]
        [TestCase(3, 3, 4, 4, 5, Category.Threes, 6)]
        [TestCase(4, 4, 4, 4, 4, Category.Fours, 20)]
        [TestCase(4, 4, 5, 5, 6, Category.Fours, 8)]
        [TestCase(5, 5, 5, 5, 5, Category.Fives, 25)]
        [TestCase(5, 5, 6, 6, 1, Category.Fives, 10)]
        [TestCase(6, 6, 6, 6, 6, Category.Sixes, 30)]
        [TestCase(6, 6, 1, 1, 2, Category.Sixes, 12)]
        [TestCase(1, 1, 1, 2, 2, Category.ThreeOfKind, 7)]
        [TestCase(1, 1, 1, 2, 3, Category.ThreeOfKind, 8)]
        [TestCase(1, 1, 1, 1, 2, Category.FourOfKind, 6)]
        [TestCase(1, 1, 1, 1, 3, Category.FourOfKind, 7)]
        [TestCase(1, 1, 2, 2, 2, Category.FullHouse, 25)]
        [TestCase(1, 1, 2, 2, 3, Category.FullHouse, 0)]
        [TestCase(1, 2, 3, 4, 1, Category.SmallStraight, 30)]
        [TestCase(2, 3, 4, 5, 2, Category.SmallStraight, 30)]
        [TestCase(2, 3, 4, 6, 2, Category.SmallStraight, 0)]
        [TestCase(1, 2, 3, 4, 5, Category.LargeStraight, 40)]
        [TestCase(1, 2, 3, 4, 6, Category.LargeStraight, 0)]
        [TestCase(1, 1, 1, 1, 1, Category.Chance, 5)]
        [TestCase(1, 2, 1, 1, 3, Category.Chance, 8)]
        [TestCase(1, 1, 1, 1, 1, Category.Yahtzee, 50)]
        [TestCase(6, 6, 6, 6, 6, Category.Yahtzee, 50)]
        [TestCase(1, 1, 1, 1, 2, Category.Yahtzee, 0)]
        [TestCase(2, 2, 3, 1, 5, Category.Yahtzee, 0)]
        public void GetAvailableOptions_PlayerRollsFiveDices_ReturnsDictionaryWithCalculatedScoreForEachCategory(
            int die1, int die2, int die3, int die4, int die5, Category categoryToCheck, int expectedScore)
        {
            string[] playerName = { "A" };
            _randomizer.Roll(1, 6).Returns(die1, die2, die3, die4, die5);
            List<Dice> dice = MakeNewDiceSet();

            _game.NewGame(playerName);
            _game.RollDice(dice);
            var result = _game.GetAvailableOptions(playerName[0]);

            Assert.AreEqual(expectedScore, result[categoryToCheck]);
        }
    }
}
