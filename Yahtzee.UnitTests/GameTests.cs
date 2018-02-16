﻿using NUnit.Framework;
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

        private static object[] _combinations =
        {
            new object[]{1, 1, 1, 1, 1, Category.Aces, 5                     },
            new object[]{1, 1, 1, 1, 2, Category.Aces, 4                     },
            new object[]{2, 2, 2, 2, 2, Category.Twos, 10                    },
            new object[]{2, 2, 2, 2, 3, Category.Twos, 8                     },
            new object[]{3, 3, 3, 3, 3, Category.Threes, 15                  },
            new object[]{3, 3, 3, 3, 4, Category.Threes, 12                  },
            new object[]{4, 4, 4, 4, 4, Category.Fours, 20                   },
            new object[]{4, 4, 4, 4, 5, Category.Fours, 16                   },
            new object[]{5, 5, 5, 5, 5, Category.Fives, 25                   },
            new object[]{5, 5, 5, 5, 6, Category.Fives, 20                   },
            new object[]{6, 6, 6, 6, 6, Category.Sixes, 30                   },
            new object[]{6, 6, 6, 6, 1, Category.Sixes, 24                   },
            new object[]{1, 1, 1, 2, 2, Category.ThreeOfKind, 7              },
            new object[]{1, 1, 1, 2, 3, Category.ThreeOfKind, 8              },
            new object[]{1, 1, 1, 1, 2, Category.FourOfKind, 6               },
            new object[]{1, 1, 1, 1, 3, Category.FourOfKind, 7               },
            new object[]{1, 1, 2, 2, 2, Category.FourOfKind, 0               },
            new object[]{1, 1, 2, 2, 2, Category.FullHouse, 25               },
            new object[]{1, 1, 2, 2, 3, Category.FullHouse, 0                },
            new object[]{1, 2, 3, 4, 1, Category.SmallStraight, 30           },
            new object[]{2, 3, 4, 5, 2, Category.SmallStraight, 30           },
            new object[]{2, 3, 4, 6, 2, Category.SmallStraight, 0            },
            new object[]{1, 2, 3, 4, 5, Category.SmallStraight, 30           },
            new object[]{1, 2, 2, 2, 2, Category.SmallStraight, 0            },
            new object[]{1, 2, 3, 4, 5, Category.LargeStraight, 40           },
            new object[]{1, 2, 3, 4, 6, Category.LargeStraight, 0            },
            new object[]{1, 2, 2, 2, 2, Category.LargeStraight, 0            },
            new object[]{1, 1, 1, 1, 1, Category.Chance, 5                   },
            new object[]{1, 2, 1, 1, 3, Category.Chance, 8                   },
            new object[]{1, 1, 1, 1, 1, Category.Yahtzee, 50                 },
            new object[]{6, 6, 6, 6, 6, Category.Yahtzee, 50                 },
            new object[]{1, 1, 1, 1, 2, Category.Yahtzee, 0                  },
            new object[]{2, 2, 3, 1, 5, Category.Yahtzee, 0                  }
        };

        [TestCaseSource(nameof(_combinations))]
        public void AddPoints_ForGivenCategory_PointsAreStored(
            int die1, int die2, int die3, int die4, int die5, Category selectedCategory, int expectedScore)
        {
            _randomizer.Roll(1, 6).Returns(die1, die2, die3, die4, die5);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            _game.AddPoints(selectedCategory);

            var result = _game.GameStatus()[0][selectedCategory];

            Assert.AreEqual(expectedScore, result);
        }

        [Test]
        public void AddPoints_CategoryAlreadyTaken_Throws()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            TestDelegate result = () => _game.AddPoints(Category.Aces);

            Assert.Throws<ArgumentException>(result);
        }

        [Test]
        public void ActivePlayer_LastPlayerScoredPoints_FirstPlayerIsActive()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("0", "1", "2", "3");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            var result = _game.ActivePlayer;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void ActivePlayer_PlayerRecivedPointsForGivenCategory_NextPlayerIsActive()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("0", "1");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            var result = _game.ActivePlayer;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void ActivePlayer_PlayerRecivedPointsForGivenCategory_NextPlayerHas3RollsLeft()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("0", "1");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            var result = _game.RollsLeft;

            Assert.AreEqual(3, result);
        }

        [Test]
        public void GetAvailableOptions_PlayerRollsFiveDices_ReturnsDictionaryWithCalculatedScoreForEachAvailableCategory()
        {
            _randomizer.Roll(1, 6).Returns(1);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            var expected = new Dictionary<Category, int>()
            {
                { Category.Aces, 5 },
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
            var result = _game.GetAvailableOptions();

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void GetAvailableOptions_PlayerRollsFiveDices_ReturnsDictionaryWithCalculatedScoreForEachAvailableCategory2()
        {
            _randomizer.Roll(1, 6).Returns(1,1,2,2,2);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            var expected = new Dictionary<Category, int>()
            {
                { Category.Aces, 2 },
                { Category.Twos, 6 },
                { Category.Threes, 0 },
                { Category.Fours, 0 },
                { Category.Fives, 0 },
                { Category.Sixes, 0 },
                { Category.ThreeOfKind, 8 },
                { Category.FourOfKind, 0 },
                { Category.FullHouse, 25 },
                { Category.SmallStraight, 0 },
                { Category.LargeStraight, 0 },
                { Category.Chance, 8 },
                { Category.Yahtzee, 0 },
            };
            var result = _game.GetAvailableOptions();

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void GetAvailableOptions_PlayerAlreadyPickedOneCategory_ReturnsDictionaryWithCalculatedScoreForEachLeftCategory()
        {
            _randomizer.Roll(1, 6).Returns(1, 1, 2, 2, 2);
            IDice[] dice = MakeNewDiceSet();

            _game.NewGame("A");
            _game.RollDice(dice);
            _game.AddPoints(Category.Aces);

            var expected = new Dictionary<Category, int>()
            {
                { Category.Twos, 6 },
                { Category.Threes, 0 },
                { Category.Fours, 0 },
                { Category.Fives, 0 },
                { Category.Sixes, 0 },
                { Category.ThreeOfKind, 8 },
                { Category.FourOfKind, 0 },
                { Category.FullHouse, 25 },
                { Category.SmallStraight, 0 },
                { Category.LargeStraight, 0 },
                { Category.Chance, 8 },
                { Category.Yahtzee, 0 },
            };
            var result = _game.GetAvailableOptions();

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
