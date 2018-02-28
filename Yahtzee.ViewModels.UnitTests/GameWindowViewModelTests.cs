using NSubstitute;
using NUnit.Framework;
using System;
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

        [SetUp]
        public void Setup()
        {
            _randomizer = Substitute.For<IRandomizer>();
            _players = new[] { "A", "B", "C", "D" };
            _vm = new GameWindowViewModel(_randomizer, _players);
        }

        [Test]
        public void GameWindowViewModel_CanBeCreated()
        {
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, _players);
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
            ICommand rollDiceCommand = _vm.RollDiceCommand;

            rollDiceCommand.Execute(null);
            var result = _vm.RollResult;
            var expected = new[] { rollResult, rollResult, rollResult, rollResult, rollResult };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RollDiceCommand_PlayerRollsThreeTimes_CommandIsDisabled()
        {
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand rollDiceCommand = _vm.RollDiceCommand;

            rollDiceCommand.Execute(null);
            rollDiceCommand.Execute(null);
            rollDiceCommand.Execute(null);
            var result = rollDiceCommand.CanExecute(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void RollResult_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.RollResult))
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand rollDiceCommand = _vm.RollDiceCommand;

            rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void UpdateTable_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.UpdateTable))
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand rollDiceCommand = _vm.RollDiceCommand;

            rollDiceCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void GameWindowViewModel_WhenCreated_PlayersReturnsGivenPlayersNames()
        {
            var expected = new[] { "A", "B", "C", "D" };
            GameWindowViewModel vm = new GameWindowViewModel(_randomizer, expected);

            var result = vm.Players;

            Assert.AreEqual(expected, result);
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

            Assert.AreEqual("A's Turn:", result);
        }

        [Test]
        public void PickCategoryCommand_PlayerPickCategory_NextPlayerIsActive()
        {
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            var result = _vm.ActivePlayer;

            Assert.AreEqual("B's Turn:", result);
        }

        [Test]
        public void ActivePlayer_PropertyChanged_IsFired()
        {
            bool isFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.ActivePlayer))
                    isFired = true;
            };
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(isFired);
        }

        [TestCase("Aces", 1, 5)]
        [TestCase("Twos", 2, 10)]
        public void PickCategoryCommand_PlayerPicksCategory_GamestateIsUpdated(
            string choosenCategory, int rollResult, int expectedScore)
        {
            _randomizer.GetRandomNumber(1, 6).Returns(rollResult);
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute(choosenCategory);
            var parseCategory = (Category)Enum.Parse(typeof(Category), choosenCategory);
            var result = _vm.UpdateTable[0][parseCategory];

            Assert.AreEqual(expectedScore, result);
        }

        [Test]
        public void PickCategoryCommand_PlayerPicksCategory_RollDiceCommandCanBeExecuted()
        {
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            rollDiceCommand.Execute(null);
            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");
            var result = rollDiceCommand.CanExecute(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void PartialScore_InitialState_ReturnsNull()
        {
            var result = _vm.PartialScore[0];

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

            Assert.AreEqual(5,result);
        }

        [Test]
        public void PartialScore_PropertyChanged_IsFired()
        {
            bool isFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.PartialScore))
                {
                    isFired = true;
                }
            };
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(isFired);
        }

        [Test]
        public void BonusScore_InitialState_ReturnsNull()
        {
            var result = _vm.BonusScore[0];

            Assert.IsNull(result);
        }

        [Test]
        public void BonusScore_PlayerPicksAllSimpleCategoriesAndPartialScoreIsGreaterThan62_BonusScoreReturnsScore()
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
            bool isFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.BonusScore))
                {
                    isFired = true;
                }
            };
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(isFired);
        }

        [Test]
        public void TotalScore_InitialState_ReturnsNull()
        {
            var result = _vm.TotalScore[0];

            Assert.IsNull(result);
        }

        [Test]
        public void TotalScore_PlayerPicksAllCategories_TotalScoreReturnsScore()
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
            bool isFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.TotalScore))
                {
                    isFired = true;
                }
            };
            ICommand rollDiceCommand = _vm.RollDiceCommand;
            ICommand pickCategoryCommand = _vm.PickCategoryCommand;

            rollDiceCommand.Execute(null);
            pickCategoryCommand.Execute("Aces");

            Assert.IsTrue(isFired);
        }
    }
}
