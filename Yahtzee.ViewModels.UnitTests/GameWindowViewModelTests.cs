using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Yahtzee.Core;

namespace Yahtzee.ViewModels.UnitTests
{
    [TestFixture]
    public class GameWindowViewModelTests
    {
        private IRandomizer _randomizer;
        private string[] _players;
        private GameWindowViewModel _vm;
        private ICommand _rollDiceCommand;
        private ICommand _pickCategoryCommand;
        private ICommand _toggleDiceLockCommand;

        [SetUp]
        public void Setup()
        {
            _randomizer = Substitute.For<IRandomizer>();
            _players = new[] { "A", "B", "C", "D" };
            _vm = new GameWindowViewModel(_randomizer, _players);
            _rollDiceCommand = _vm.RollDiceCommand;
            _pickCategoryCommand = _vm.PickCategoryCommand;
            _toggleDiceLockCommand = _vm.ToggleDiceLockCommand;
        }

        [Test]
        public void GameWindowViewModel_CanBeCreated()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, _players);
        }

        [Test]
        public void GameWindowViewModel_WhenCreated_PlayerNamesAreStored()
        {
            var expected = new[] { "A", "B", "C", "D" };
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, expected);

            var result = vm.Players;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RollDiceCommand_CanBeExecuted()
        {
            ICommand rollDiceCommand = _vm.RollDiceCommand;

            rollDiceCommand.Execute(null);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void RollDiceCommand_CommandExecuted_ReturnsRollResults(
            int rollResult)
        {
            _randomizer.GetRandomNumber(1, 6).Returns(rollResult);

            _rollDiceCommand.Execute(null);
            var expected = rollResult;
            var result = _vm.Dice;

            Assert.AreEqual(expected, result[0].Result);
            Assert.AreEqual(expected, result[1].Result);
            Assert.AreEqual(expected, result[2].Result);
            Assert.AreEqual(expected, result[3].Result);
            Assert.AreEqual(expected, result[4].Result);
        }

        [Test]
        public void RollDiceCommand_PlayerRollsThreeTimes_CommandIsDisabled()
        {
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            var result = _rollDiceCommand.CanExecute(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void RollDiceCommand_CommandExecuted_ScoreTableReturnsAvailableCategoriesWithScores()
        {
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            _rollDiceCommand.Execute(null);
            var expected = new Dictionary<Category, int?>
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
            var result = _vm.ScoreTable[0];

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RollDiceCommand_CommandExecutedAfterOneCategoryAlreadyPicked_ScoreTableReturnsAvailableCategoriesCombinedWithPreviousScores()
        {
            var vm = new GameWindowViewModel(_randomizer, "A");
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            var expected = new Dictionary<Category, int?>
            {
                    { Category.Aces, 5 },
                    { Category.Twos, 10 },
                    { Category.Threes, 0 },
                    { Category.Fours, 0 },
                    { Category.Fives, 0 },
                    { Category.Sixes, 0 },
                    { Category.ThreeOfKind, 10 },
                    { Category.FourOfKind, 10 },
                    { Category.FullHouse, 0 },
                    { Category.SmallStraight, 0 },
                    { Category.LargeStraight, 0 },
                    { Category.Chance, 10 },
                    { Category.Yahtzee, 50 },
            };
            var result = vm.ScoreTable[0];

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ScoreTable_CollectionChanged_IsFired()
        {
            bool hasFired = false;
            _vm.ScoreTable.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != args.OldItems)
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            _rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void ScoreTable_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.ScoreTable))
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void PickCategoryCommand_CanBeExecuted()
        {
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
        }

        [Test]
        public void ActivePlayer_InitialState_ReturnsPlayerOneName()
        {
            var result = _vm.ActivePlayer;

            Assert.AreEqual("A", result);
        }

        [Test]
        public void PickCategoryCommand_CommandExecuted_NextPlayerIsActive()
        {
            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");
            var result = _vm.ActivePlayer;

            Assert.AreEqual("B", result);
        }

        [Test]
        public void ActivePlayer_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.ActivePlayer))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [TestCase("Aces", 1, 5)]
        [TestCase("Twos", 2, 10)]
        public void PickCategoryCommand_CommandExecuted_ScoreTableIsUpdated(
            string choosenCategory, int rollResult, int expectedScore)
        {
            _randomizer.GetRandomNumber(1, 6).Returns(rollResult);

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute(choosenCategory);
            var parseCategory = (Category)Enum.Parse(typeof(Category), choosenCategory);
            var result = _vm.ScoreTable[0][parseCategory];
            var result2 = _vm.ScoreTable[0][Category.Sixes];

            Assert.AreEqual(expectedScore, result);
            Assert.AreEqual(null, result2);
        }

        [Test]
        public void PickCategoryCommand_PlayerPicksCategoryAfterThreeRolls_RollDiceCommandCanBeExecuted()
        {
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");
            var result = _rollDiceCommand.CanExecute(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void PartialScore_InitialState_ReturnsNull()
        {
            var result = _vm.PartialScore;

            Assert.IsNull(result);
        }

        [Test]
        public void PickCategoryCommand_PlayerPicksAllSimpleCategories_PartialScoreReturnsScore()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            var result = vm.PartialScore[0];

            Assert.AreEqual(5, result);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_InitialState_ReturnsFalse()
        {
            var result = _vm.IsPickCategoryCommandAvailable.All(x => x.All(y => y.Value == true));

            Assert.IsFalse(result);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_RollDiceCommandExecuted_ReturnsTrue()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;

            rollDiceCommand.Execute(null);
            var result = vm.IsPickCategoryCommandAvailable.All(x => x.All(y => y.Value == true));

            Assert.IsTrue(result);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_OneCategoryTaken_ReturnsTrueForOtherCategory()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            rollDiceCommand.Execute(null);
            var result1 = vm.IsPickCategoryCommandAvailable[0][Category.Aces];
            var result2 = vm.IsPickCategoryCommandAvailable[0][Category.Twos];

            Assert.IsFalse(result1);
            Assert.IsTrue(result2);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_RollDiceCommandExecutedAfterOneCategoryTaken_ReturnsFalseForThatCategory()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A", "B");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            var result = vm.IsPickCategoryCommandAvailable[0][Category.Aces];

            Assert.IsFalse(result);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_RollDiceCommandExecuted_PropertyChangedIsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.IsPickCategoryCommandAvailable))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void IsPickCategoryCommandAvailable_PickCategoryCommandExecuted_PropertyChangedIsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.IsPickCategoryCommandAvailable))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void PartialScore_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.PartialScore))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void BonusScore_InitialState_ReturnsNull()
        {
            var result = _vm.BonusScore;

            Assert.IsNull(result);
        }

        [Test]
        public void BonusScore_PlayerPicksAllSimpleCategoriesAndScoreIsGreaterThan62_ReturnsBonusScore()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            _randomizer.GetRandomNumber(1, 6).Returns(3);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            _randomizer.GetRandomNumber(1, 6).Returns(4);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            _randomizer.GetRandomNumber(1, 6).Returns(5);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            _randomizer.GetRandomNumber(1, 6).Returns(6);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            var result = vm.BonusScore[0];

            Assert.AreEqual(35, result);
        }

        [Test]
        public void BonusScore_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.BonusScore))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void TotalScore_InitialState_ReturnsNull()
        {
            var result = _vm.TotalScore;

            Assert.IsNull(result);
        }

        [Test]
        public void TotalScore_AllCategoriesAreTaken_ReturnsTotalScore()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("ThreeOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FourOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FullHouse");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("SmallStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("LargeStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Chance");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Yahtzee");
            var result = vm.TotalScore[0];

            Assert.AreEqual(20 + 50, result);
        }

        [Test]
        public void TotalScore_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.TotalScore))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Dice_InitialResultValue_Returns0(
            int dieNumber)
        {
            var result = _vm.Dice[dieNumber].Result;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void Dice_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.Dice))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void ToggleDiceLock_CanBeExecuted()
        {
            ICommand toggleDiceLockCommand = _vm.ToggleDiceLockCommand;

            toggleDiceLockCommand.Execute("0");
        }

        [Test]
        public void ToggleDiceLock_GivenDiceNumberToLock_DiceIsLocked()
        {
            _rollDiceCommand.Execute(null);
            _toggleDiceLockCommand.Execute("0");
            var result = _vm.Dice[0].IsUnlocked;

            Assert.IsFalse(result);
        }

        [Test]
        public void ToggleDiceLock_GivenDiceNumberToUnlock_DiceIsUnlocked()
        {
            _toggleDiceLockCommand.Execute("0");
            _toggleDiceLockCommand.Execute("0");
            var result = _vm.Dice[0].IsUnlocked;

            Assert.IsTrue(result);
        }

        [Test]
        public void ToggleDiceLock_PlayerTriesToLockDiceBeforeFirstRoll_DiceStaysUnlocked()
        {
            _toggleDiceLockCommand.Execute("0");
            var result = _vm.Dice[0].IsUnlocked;

            Assert.IsTrue(result);
        }

        [Test]
        public void PickCategoryCommand_CommandExecuted_UnlocksAllDiceForNextRound()
        {
            _rollDiceCommand.Execute(null);
            _toggleDiceLockCommand.Execute("0");
            _pickCategoryCommand.Execute("Aces");
            var result = new[]
            {
                _vm.Dice[0].IsUnlocked,
                _vm.Dice[1].IsUnlocked,
                _vm.Dice[2].IsUnlocked,
                _vm.Dice[3].IsUnlocked,
                _vm.Dice[4].IsUnlocked
            };

            Assert.IsTrue(result[0]);
            Assert.IsTrue(result[1]);
            Assert.IsTrue(result[2]);
            Assert.IsTrue(result[3]);
            Assert.IsTrue(result[4]);
        }

        [Test]
        public void Dice_ToggleDiceCommandExecuted_PropertyChangedIsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.Dice))
                    hasFired = true;
            };

            _toggleDiceLockCommand.Execute("0");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void Dice_PickCategoryCommandExecuted_PropertyChangedIsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.Dice))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void MessageInfo_InitialState_ReturnsWelcomeText()
        {
            var result = _vm.MessageInfo;
            var expected = "Press \"Roll Dice\" button to begin!";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void MessageInfo_RollDiceExecuted_ReturnsInfo()
        {
            _rollDiceCommand.Execute(null);
            var result = _vm.MessageInfo;
            var expected = "Available options:\n" +
                "Pick dice you want to keep and roll the remaining ones.\n" +
                "Pick available category on table to gain points.\n" +
                "Rolls left: 2.";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void MessageInfo_RollDiceExecuted2Times_ReturnsInfo()
        {
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            var result = _vm.MessageInfo;
            var expected = "Available options:\n" +
                "Pick dice you want to keep and roll the remaining ones.\n" +
                "Pick available category on table to gain points.\n" +
                "Rolls left: 1.";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void MessageInfo_RollDiceExecutedAndNoMoreRollsLeft_ReturnsInfo()
        {
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            _rollDiceCommand.Execute(null);
            var result = _vm.MessageInfo;
            var expected = "Available options:\n" +
                "Pick available category on table to gain points.";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void MessageInfo_RollDiceCommandExecuted_PropertyChangedIsFired()
        {
            var hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.MessageInfo))
                    hasFired = true;
            };

            _rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void MessageInfo_PickCategoryCommandExecuted_ReturnsInfo()
        {
            _rollDiceCommand.Execute(null);
            _pickCategoryCommand.Execute("Aces");
            var result = _vm.MessageInfo;
            var expected = "Press \"Roll Dice\" button to begin!";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void RollDiceCommand_GameIsOver_CommandIsDisabled()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("ThreeOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FourOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FullHouse");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("SmallStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("LargeStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Chance");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Yahtzee");

            var result = rollDiceCommand.CanExecute(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void MessageInfo_GameIsOverForOnePlayer_ReturnsWinner()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;
            _randomizer.GetRandomNumber(1, 6).Returns(1);

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("ThreeOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FourOfKind");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FullHouse");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("SmallStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("LargeStraight");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Chance");
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Yahtzee");
            var expected = "Player A is the winner!";
            var result = vm.MessageInfo;

            StringAssert.AreEqualIgnoringCase(expected, result);
        }

        [Test]
        public void MessageInfo_GameIsOverForTwoPlayers_ReturnsWinner()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, "A", "B");
            ICommand rollDiceCommand = vm.RollDiceCommand;
            ICommand pickCategoryCommand = vm.PickCategoryCommand;

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Twos");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Threes");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fours");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Fives");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Sixes");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("ThreeOfKind");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("ThreeOfKind");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FourOfKind");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FourOfKind");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FullHouse");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("FullHouse");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("SmallStraight");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("SmallStraight");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("LargeStraight");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("LargeStraight");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Chance");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Chance");

            _randomizer.GetRandomNumber(1, 6).Returns(1);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Yahtzee");
            _randomizer.GetRandomNumber(1, 6).Returns(2);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Yahtzee");
            var expected = "Player B is the winner!";
            var result = vm.MessageInfo;

            StringAssert.AreEqualIgnoringCase(expected, result);
        }
    }
}
