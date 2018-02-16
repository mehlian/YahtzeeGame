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

        private IDice[] MakeNewDiceSet()
        {
            return new[] { new Dice(), new Dice(), new Dice(), new Dice(), new Dice() };
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

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void RollDice_GivenUnlockedDices_RollResultsAreStored(int expected)
        {
            _randomizer.Roll(1, 6).Returns(expected);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            var result = _game.RollResult.All(d => d.Result == expected);

            Assert.IsTrue(result);
        }

        [Test]
        public void RollDice_GivenLockedDices_ReturnsOldResults()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            dice.All(d =>
            {
                d.Lock();
                return true;
            });
            _randomizer.Roll(1, 6).Returns(2);
            _game.RollDice(dice);

            var result = _game.RollResult.All(d => d.Result == 1);

            Assert.IsTrue(result);
        }

        [Test]
        public void RollDice_GivenListOfDicesThatIsNotEqualTo5_Throws()
        {
            IDice[] dice = new[] { new Dice() };

            _game.NewGame("A");
            TestDelegate result = () => _game.RollDice(dice);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void NewGame_PlayerNameAsString_PlayerIsAddedToTheGame()
        {
            string[] playerName = { "A" };
            _game.NewGame(playerName);

            var result = _game.Players.All(x => x == "A");

            Assert.IsTrue(result);
        }

        [Test]
        public void NewGame_PlayerNamesAsStringArray_PlayersAreAdded()
        {
            string[] playersName = { "A", "B", "C", "D" };
            _game.NewGame(playersName);

            Assert.AreEqual(playersName, _game.Players);
        }

        [Test]
        public void NewGame_ArrayOfPlayerNamesIsLongerThan4_Throws()
        {
            string[] playersName = { "1", "2", "3", "4", "5" };
            TestDelegate result = () => _game.NewGame(playersName);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void NewGame_ArrayOfPlayersIsAdded_FirstPlayerFromArrayIsActive()
        {
            string[] playersName = { "A", "B", "C", "D" };
            _game.NewGame(playersName);
            var result = _game.ActivePlayer;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void RollsLeft_InitialRollLimitForActivePlayer_Returns3()
        {
            _game.NewGame("A");

            var result = _game.RollsLeft;

            Assert.AreEqual(3, result);
        }

        [Test]
        public void RollsLeft_PLayerRollsThreeTimes_Returns0()
        {
            _game.NewGame("A");
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.RollDice(dice);
            _game.RollDice(dice);
            _game.RollDice(dice);

            var result = _game.RollsLeft;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void RollDice_WhenNoMoreRollsLeft_Throws()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            _game.RollDice(dice);
            _game.RollDice(dice);

            TestDelegate result = () => _game.RollDice(dice);

            Assert.Throws<InvalidOperationException>(result);
        }

        [Test]
        public void GameStatus_PointsForAllCategoriesAtInitialStateForOnePlayer_ReturnsNull()
        {
            _game.NewGame("A");

            var result = _game.GameStatus().All(x => x.Values.All(y => y.HasValue == false));

            Assert.IsTrue(result);
        }

        [Test]
        public void GameStatus_PointsForAllCategoriesAtInitialStateForTwoPlayers_ReturnsNull()
        {
            _game.NewGame("A", "B");

            var result = _game.GameStatus().All(x => x.Values.All(y => y.HasValue == false));

            Assert.IsTrue(result);
        }

        [TestCase(1, 1, 1, 1, 1, Category.Aces, 5)]
        [TestCase(1, 1, 1, 1, 2, Category.Aces, 4)]
        [TestCase(2, 2, 2, 2, 2, Category.Twos, 10)]
        [TestCase(2, 2, 2, 2, 3, Category.Twos, 8)]
        [TestCase(3, 3, 3, 3, 3, Category.Threes, 15)]
        [TestCase(3, 3, 3, 3, 4, Category.Threes, 12)]
        [TestCase(4, 4, 4, 4, 4, Category.Fours, 20)]
        [TestCase(4, 4, 4, 4, 5, Category.Fours, 16)]
        [TestCase(5, 5, 5, 5, 5, Category.Fives, 25)]
        [TestCase(5, 5, 5, 5, 6, Category.Fives, 20)]
        [TestCase(6, 6, 6, 6, 6, Category.Sixes, 30)]
        [TestCase(6, 6, 6, 6, 1, Category.Sixes, 24)]
        [TestCase(1, 1, 1, 2, 2, Category.ThreeOfKind, 7)]
        [TestCase(1, 1, 1, 2, 3, Category.ThreeOfKind, 8)]
        [TestCase(1, 1, 1, 1, 2, Category.FourOfKind, 6)]
        [TestCase(1, 1, 1, 1, 3, Category.FourOfKind, 7)]
        [TestCase(1, 1, 2, 2, 2, Category.FullHouse, 25)]
        [TestCase(1, 1, 2, 2, 3, Category.FullHouse, 0)]
        [TestCase(1, 2, 3, 4, 1, Category.SmallStraight, 30)]
        [TestCase(2, 3, 4, 5, 2, Category.SmallStraight, 30)]
        [TestCase(2, 3, 4, 6, 2, Category.SmallStraight, 0)]
        [TestCase(1, 2, 3, 4, 5, Category.SmallStraight, 30)]
        [TestCase(1, 2, 2, 2, 2, Category.SmallStraight, 0)]
        [TestCase(1, 2, 3, 4, 5, Category.LargeStraight, 40)]
        [TestCase(1, 2, 3, 4, 6, Category.LargeStraight, 0)]
        [TestCase(1, 2, 2, 2, 2, Category.LargeStraight, 0)]
        [TestCase(1, 1, 1, 1, 1, Category.Chance, 5)]
        [TestCase(1, 2, 1, 1, 3, Category.Chance, 8)]
        [TestCase(1, 1, 1, 1, 1, Category.Yahtzee, 50)]
        [TestCase(6, 6, 6, 6, 6, Category.Yahtzee, 50)]
        [TestCase(1, 1, 1, 1, 2, Category.Yahtzee, 0)]
        [TestCase(2, 2, 3, 1, 5, Category.Yahtzee, 0)]
        public void AddPoints_ForGivenCategory_PointsAreStored(
            int die1, int die2, int die3, int die4, int die5, Category selectedCategory, int expectedScore)
        {
            _randomizer.Roll(1, 6).Returns(die1, die2, die3, die4, die5);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            _game.AddPoints(selectedCategory);

            var result = _game.GameStatus()[_game.ActivePlayer][selectedCategory];

            Assert.AreEqual(expectedScore, result);
        }

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
        [TestCase(1, 2, 3, 4, 5, Category.SmallStraight, 30)]
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
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame(playerName);
            _game.RollDice(dice);
            var result = _game.GetAvailableOptions(playerName[0]);

            Assert.AreEqual(expectedScore, result[categoryToCheck]);
        }


    }
}
