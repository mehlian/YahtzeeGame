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
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 6;
        private IRandomizer _randomizer;
        private Game _game;

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

        [TestCase]
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
        public void RollDice_GivenUnlockedDices_ResultsAreStored(int expectedRollResult)
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(expectedRollResult);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");

            _game.RollDice(dices);
            var result = _game.RollResult.All(d => d.Result == expectedRollResult);

            Assert.IsTrue(result);
        }

        [TestCase]
        public void RollDice_GivenLockedDices_ResultsAreNotStored()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1); //first roll result
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            dices.ToList().ForEach(d => d.Lock());
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(2); //second roll result

            _game.RollDice(dices);
            var result = _game.RollResult.All(d => d.Result == 1);

            Assert.IsTrue(result);
        }

        [TestCase]
        public void RollDice_GivenDiceSetNotEqualToFive_Throws()
        {
            var dices = new IDice[1];
            _game.NewGame("A");

            TestDelegate result = () => _game.RollDice(dices);

            Assert.Throws<RankException>(result);
        }

        [TestCase("A")]
        [TestCase("A", "B", "C", "D")]
        public void NewGame_GivenPlayersNames_NamesAreStored(params string[] playersNames)
        {
            _game.NewGame(playersNames);
            var result = _game.Players;

            Assert.AreEqual(playersNames, result);
        }

        [TestCase("1", "2", "3", "4", "5")]
        public void NewGame_GivenMorePlayersNamesThanFour_Throws(params string[] playersNames)
        {
            TestDelegate result = () => _game.NewGame(playersNames);

            Assert.Throws<ArgumentException>(result);
        }

        [TestCase(0, "A", "B", "C", "D")]
        public void NewGame_GivenPlayersNames_FirstAddedPlayerIsActivePlayer(int expectedActivePlayer, params string[] playersNames)
        {
            _game.NewGame(playersNames);
            var result = _game.ActivePlayer;

            Assert.AreEqual(expectedActivePlayer, result);
        }

        [TestCase]
        public void NewGame_InitialNumberOfRollsAvailable_ReturnsThree()
        {
            _game.NewGame("A");
            var result = _game.RollsLeft;

            Assert.AreEqual(3, result);
        }

        [TestCase]
        public void RollDice_PLayerRollsThreeTimes_NoMoreRollsAvailable()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");

            _game.RollDice(dices);
            _game.RollDice(dices);
            _game.RollDice(dices);
            var result = _game.RollsLeft;

            Assert.AreEqual(0, result);
        }

        [TestCase]
        public void RollDice_PlayerRollsForthTime_Throws()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");

            _game.RollDice(dices);
            _game.RollDice(dices);
            _game.RollDice(dices);
            TestDelegate result = () => _game.RollDice(dices);

            Assert.Throws<InvalidOperationException>(result);
        }

        [TestCase("A")]
        [TestCase("A", "B")]
        public void NewGame_InitialValuesForAllCategories_GameStatusReturnsInitialValues(params string[] playersNames)
        {
            int? initialValue = null;

            _game.NewGame("A");
            var result = _game.GameStatus().All(dict => dict.Values.All(category => category == initialValue));

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
        [TestCase(1, 1, 2, 2, 2, Category.FourOfKind, 0)]
        [TestCase(1, 1, 2, 2, 2, Category.FullHouse, 25)]
        [TestCase(1, 1, 2, 2, 3, Category.FullHouse, 0)]
        [TestCase(2, 3, 2, 2, 2, Category.FullHouse, 0)]
        [TestCase(2, 6, 4, 3, 1, Category.SmallStraight, 30)]
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
        public void AddPoints_GivenCategory_PointsAreStored(
            int die1, int die2, int die3, int die4, int die5, Category selectedCategory, int expectedScore)
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(die1, die2, die3, die4, die5);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);

            _game.AddPoints(selectedCategory);
            var result = _game.GameStatus().First()[selectedCategory];

            Assert.AreEqual(expectedScore, result);
        }

        [TestCase]
        public void AddPoints_ChosenCategoryAlreadyTaken_Throws()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            TestDelegate result = () => _game.AddPoints(Category.Aces);

            Assert.Throws<ArgumentException>(result);
        }

        [TestCase]
        public void AddPoints_WhenPlayerChooseCategory_NextPlayerIsActive()
        {
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();
            _game.NewGame("0", "1");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            var result = _game.ActivePlayer;

            Assert.AreEqual(1, result);
        }

        [TestCase]
        public void AddPoints_WhenLastPlayerChooseCategory_FirstPlayerIsActive()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("0", "1", "2", "3");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dices);

            _game.AddPoints(Category.Aces);
            var result = _game.ActivePlayer;

            Assert.AreEqual(0, result);
        }

        [TestCase]
        public void AddPoints_WhenPlayerChooseCategory_NextPlayersHasThreeRollsLeft()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("0", "1");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var result = _game.RollsLeft;

            Assert.AreEqual(3, result);
        }

        [TestCase(new[] { 1, 1, 1, 1, 1 }, new[] { 5, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 5, 50 })]
        [TestCase(new[] { 1, 1, 2, 2, 2 }, new[] { 2, 6, 0, 0, 0, 0, 8, 0, 25, 0, 0, 8, 0 })]
        public void GetAvailableCategories_WhenPlayerRolls_ReturnsCalculatedScoreForEachAvailableCategory(
            int[] rollResult, int[] expectedScore)
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(rollResult.First(), rollResult.Skip(1).ToArray());
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);

            var expected = new Dictionary<Category, int>()
            {
                { Category.Aces, expectedScore[0] },
                { Category.Twos, expectedScore[1] },
                { Category.Threes, expectedScore[2] },
                { Category.Fours, expectedScore[3] },
                { Category.Fives, expectedScore[4] },
                { Category.Sixes, expectedScore[5] },
                { Category.ThreeOfKind, expectedScore[6] },
                { Category.FourOfKind, expectedScore[7] },
                { Category.FullHouse, expectedScore[8] },
                { Category.SmallStraight, expectedScore[9] },
                { Category.LargeStraight, expectedScore[10] },
                { Category.Chance, expectedScore[11] },
                { Category.Yahtzee, expectedScore[12] },
            };
            var result = _game.GetAvailableCategories();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GetAvailableCategories_OneCategoryPicked_ReturnsCalculatedScoreForRemainingCategories()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var expected = new Dictionary<Category, int?>()
            {
                { Category.Aces, null },
                { Category.Twos, 0 },
                { Category.Threes, 0 },
                { Category.Fours, 0 },
                { Category.Fives, 0 },
                { Category.Sixes, 0 },
                { Category.ThreeOfKind, 5 },
                { Category.FourOfKind, 5 },
                { Category.FullHouse, 0 },
                { Category.SmallStraight, 0 },
                { Category.LargeStraight, 0 },
                { Category.Chance, 5 },
                { Category.Yahtzee, 50 },
            };
            var result = _game.GetAvailableCategories();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GameStatus_WhenNoPlayersNamesGiven_ReturnsNull()
        {
            var result = _game.GameStatus();

            Assert.AreEqual(null, result);
        }

        [TestCase]
        public void GameStatus_WhenPlayersNamesGiven_ReturnsDefaultValue()
        {
            _game.NewGame("A");

            var expected = new Dictionary<Category, int?>[]
            {
                new Dictionary<Category, int?>()
                {
                    { Category.Aces, null },
                    { Category.Twos, null },
                    { Category.Threes, null },
                    { Category.Fours, null },
                    { Category.Fives, null },
                    { Category.Sixes, null },
                    { Category.ThreeOfKind, null },
                    { Category.FourOfKind, null },
                    { Category.FullHouse, null },
                    { Category.SmallStraight, null },
                    { Category.LargeStraight, null },
                    { Category.Chance, null },
                    { Category.Yahtzee, null },
                }
            };
            var result = _game.GameStatus();

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void GameStatus_PlayerChooseOneCategory_SelectionIsStored()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var expected = new Dictionary<Category, int?>[]
            {
                new Dictionary<Category, int?>()
                {
                    { Category.Aces, 5 },
                    { Category.Twos, null },
                    { Category.Threes, null },
                    { Category.Fours, null },
                    { Category.Fives, null },
                    { Category.Sixes, null },
                    { Category.ThreeOfKind, null },
                    { Category.FourOfKind, null },
                    { Category.FullHouse, null },
                    { Category.SmallStraight, null },
                    { Category.LargeStraight, null },
                    { Category.Chance, null },
                    { Category.Yahtzee, null },
                }
            };
            var result = _game.GameStatus();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GameStatus_PlayerChooseOneCategoryAndRolls_NextRollResultIsNotStored()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dices);

            var expected = new Dictionary<Category, int?>[]
            {
                new Dictionary<Category, int?>()
                {
                    { Category.Aces, 5 },
                    { Category.Twos, null },
                    { Category.Threes, null },
                    { Category.Fours, null },
                    { Category.Fives, null },
                    { Category.Sixes, null },
                    { Category.ThreeOfKind, null },
                    { Category.FourOfKind, null },
                    { Category.FullHouse, null },
                    { Category.SmallStraight, null },
                    { Category.LargeStraight, null },
                    { Category.Chance, null },
                    { Category.Yahtzee, null },
                }
            };
            var result = _game.GameStatus();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void BonusScore_NotAllSimpleCategoriesAreTaken_ReturnsNull()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var expected = new int?[] { null };
            var result = _game.BonusScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void BonusScore_AllSimpleCategoriesAreTakenAndSumOfTheirPointsIsGreaterThan62_Returns35()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 15 points
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 20 points
            _randomizer.GetRandomNumber(1, 6).Returns(5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 25 points
            _randomizer.GetRandomNumber(1, 6).Returns(6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 30 points
                                             // Sum: 105 points

            var expected = new int?[] { 35 };
            var result = _game.BonusScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void BonusScore_AllSimpleCategoriesAreTakenAndSumOfTheirPointsIsEqualTo63_Returns35()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 2 points
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 15 points
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 20 points
            _randomizer.GetRandomNumber(1, 6).Returns(5, 5, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points
                                             // Sum: 63 points

            var expected = new int?[] { 35 };
            var result = _game.BonusScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void BonusScore_AllSimpleCategoriesAreTakenAndSumOfTheirPointsIsSmallerThan63_Returns0()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 1 points
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 15 points
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 20 points
            _randomizer.GetRandomNumber(1, 6).Returns(5, 5, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points
                                             // Sum: 62 points

            var expected = new int?[] { 0 };
            var result = _game.BonusScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void PartialScore_InitialValue_ReturnsNull()
        {
            _game.NewGame("A");

            var expected = new int?[] { null };
            var result = _game.PartialScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void PartialScore_NotAllSimpleCategoriesAreTaken_ReturnsNull()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var expected = new int?[] { null };
            var result = _game.PartialScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void PartialScore_AllSimpleCategoriesAreTaken_ReturnsSum()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 1 point
            _randomizer.GetRandomNumber(1, 6).Returns(2, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 2 points
            _randomizer.GetRandomNumber(1, 6).Returns(3, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 3 points
            _randomizer.GetRandomNumber(1, 6).Returns(4, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 4 points
            _randomizer.GetRandomNumber(1, 6).Returns(5, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points
                                             // Sum: 21 points

            var expected = new int?[] { 21 };
            var result = _game.PartialScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void PartialScore_AllSimpleCategoriesAreTakenInTwoPlayersGame_ReturnsSumForEachPlayer()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A", "B");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 1 point for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(2, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 2 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 10 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(3, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 3 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(3, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 15 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(4, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 4 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(4, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 20 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(5, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(5, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 25 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes);
            // 6 points for player A
            // Sum: 21 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(6, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes);
            // 30 points for player B
            // Sum: 140 points for player B

            var expected = new int?[] { 21, 140 };
            var result = _game.PartialScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void PartialScore_AllSimpleCategoriesAreTakenAndSumOfTheirPointsIsGreaterThan62_ReturnsSumWithBonus()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 15 points
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 20 points
            _randomizer.GetRandomNumber(1, 6).Returns(5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 25 points
            _randomizer.GetRandomNumber(1, 6).Returns(6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 30 points
                                             // Sum: 105 points

            var expected = new int?[] { 140 };
            var result = _game.PartialScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void TotalScore_NotAllCategoriesAreTaken_ReturnsNull()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);

            var expected = new int?[] { null };
            var result = _game.TotalScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void TotalScore_AllCategoriesAreTaken_ReturnsTotalScore()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 1 point
            _randomizer.GetRandomNumber(1, 6).Returns(2, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 2 points
            _randomizer.GetRandomNumber(1, 6).Returns(3, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 3 points
            _randomizer.GetRandomNumber(1, 6).Returns(4, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 4 points
            _randomizer.GetRandomNumber(1, 6).Returns(5, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points
                                             // Sum: 21 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse); // 25 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight); // 30 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight); // 40 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee); // 50 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance); // 5 points

            var expected = new int?[] { 181 };
            var result = _game.TotalScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void TotalScore_AllCategoriesAreTakenAndSimpleCategoriesSumIsGreaterThan62_ReturnsTotalScoreWithBonus()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos); // 10 points
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes); // 15 points
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours); // 20 points
            _randomizer.GetRandomNumber(1, 6).Returns(5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives); // 25 points
            _randomizer.GetRandomNumber(1, 6).Returns(6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 30 points
                                             // Sum: 105 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind); // 5 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse); // 25 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight); // 30 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight); // 40 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee); // 50 points
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance); // 5 points

            var expected = new int?[] { 300 };
            var result = _game.TotalScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void TotalScore_AllCategoriesAreTakenAndSimpleCategoriesSumIsGreaterThan62InTwoPlayersGame_ReturnsTotalScoreWithBonus()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A", "B");
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 1 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(2, 2, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 10 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 2 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(3, 3, 3, 3, 3);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 15 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(3, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 3 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(4, 4, 4, 4, 4);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 20 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(4, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 4 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(5, 5, 5, 5, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 25 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(5, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(6, 6, 6, 6, 6);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 30 points for player A
                                             // Sum: 105 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(6, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points for player B
                                             // Sum: 21 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse);
            // 25 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 2, 2, 2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse);
            // 25 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight);
            // 30 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight);
            // 30 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight);
            // 40 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 2, 3, 4, 5);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight);
            // 40 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            // 50 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            // 50 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance);
            // 5 points for player B

            var expected = new int?[] { 300, 181 };
            var result = _game.TotalScore;

            Assert.AreEqual(expected, result);
        }

        [TestCase]
        public void GameWinner_InitialState_ReturnsNull()
        {
            var result = _game.GameWinner;

            Assert.IsNull(result);
        }

        [TestCase]
        public void GameWinner_GameIsOver_ReturnsWinnersName()
        {
            var dices = MakeNewDiceSet();
            _game.NewGame("A", "B");
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            // 1 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 10 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Twos);
            // 2 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 15 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Threes);
            // 3 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 20 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fours);
            // 4 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 25 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Fives);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 30 points for player A
                                             // Sum: 105 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Sixes); // 6 points for player B
                                             // Sum: 21 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.ThreeOfKind);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FourOfKind);
            // 5 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse);
            // 25 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.FullHouse);
            // 25 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight);
            // 30 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.SmallStraight);
            // 30 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight);
            // 40 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.LargeStraight);
            // 40 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            // 50 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            // 50 points for player B
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance);
            // 5 points for player A
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance);
            // 5 points for player B

            var result = _game.GameWinner;

            StringAssert.AreEqualIgnoringCase("B", result);
        }

        [TestCase]
        public void GetAvailableCategories_RolledYahtzeeTwice_ReturnsCalculatedScoreOnlyForSimpleCategory()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            _game.RollDice(dices);

            var expected = new Dictionary<Category, int?>()
            {
                { Category.Aces, 5 },
                { Category.Twos, null },
                { Category.Threes, null },
                { Category.Fours, null },
                { Category.Fives, null },
                { Category.Sixes, null },
                { Category.ThreeOfKind, null },
                { Category.FourOfKind, null },
                { Category.FullHouse, null },
                { Category.SmallStraight, null },
                { Category.LargeStraight, null },
                { Category.Chance, null },
                { Category.Yahtzee, null },
            };
            var result = _game.GetAvailableCategories();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GetAvailableCategories_RolledYahtzeeTwicePickedSimleCategoryAndRolledDifferentYahtzee_ReturnsCalculatedScoreOnlyForSimpleCategory()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(2);
            _game.RollDice(dices);

            var expected = new Dictionary<Category, int?>()
            {
                { Category.Aces, null },
                { Category.Twos, 10 },
                { Category.Threes, null },
                { Category.Fours, null },
                { Category.Fives, null },
                { Category.Sixes, null },
                { Category.ThreeOfKind, null },
                { Category.FourOfKind, null },
                { Category.FullHouse, null },
                { Category.SmallStraight, null },
                { Category.LargeStraight, null },
                { Category.Chance, null },
                { Category.Yahtzee, null },
            };
            var result = _game.GetAvailableCategories();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GetAvailableCategories_OneCategoryPickedThenRolledYahtzeeTwice_ReturnsCalculatedScoreForRemainingCategories()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1, 2, 3, 4, 5);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            _game.RollDice(dices);

            var expected = new Dictionary<Category, int?>()
            {
                { Category.Aces, null },
                { Category.Twos, 0 },
                { Category.Threes, 0 },
                { Category.Fours, 0 },
                { Category.Fives, 0 },
                { Category.Sixes, 0 },
                { Category.ThreeOfKind, 5 },
                { Category.FourOfKind, 5 },
                { Category.FullHouse, 25 },
                { Category.SmallStraight, 30 },
                { Category.LargeStraight, 40 },
                { Category.Chance, 5 },
                { Category.Yahtzee, null },
            };
            var result = _game.GetAvailableCategories();

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase]
        public void GameStatus_PlayerGets2ndYahtzeeAfterSimpleCategoryTaken_YahtzeeScoreIsIncreasedBy100()
        {
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1, 2, 3, 4, 5);
            var dices = MakeNewDiceSet();
            _game.NewGame("A");
            _game.RollDice(dices);
            _game.AddPoints(Category.Aces);
            _randomizer.GetRandomNumber(MIN_VALUE, MAX_VALUE).Returns(1);
            _game.RollDice(dices);
            _game.AddPoints(Category.Yahtzee);
            _game.RollDice(dices);
            _game.AddPoints(Category.Chance);

            var expected = new Dictionary<Category, int?>[]
            {
                new Dictionary<Category, int?>()
                {
                    { Category.Aces, 1 },
                    { Category.Twos, null },
                    { Category.Threes, null },
                    { Category.Fours, null },
                    { Category.Fives, null },
                    { Category.Sixes, null },
                    { Category.ThreeOfKind, null },
                    { Category.FourOfKind, null },
                    { Category.FullHouse, null },
                    { Category.SmallStraight, null },
                    { Category.LargeStraight, null },
                    { Category.Chance, 5 },
                    { Category.Yahtzee, 150 },
                }
            };
            var result = _game.GameStatus();

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
