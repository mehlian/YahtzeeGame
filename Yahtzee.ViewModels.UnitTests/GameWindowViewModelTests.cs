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

        [SetUp]
        public void Setup()
        {
            _randomizer = Substitute.For<IRandomizer>();
            _players = new[] { "A", "B", "C", "D" };
        }

        public GameWindowViewModel CreateGameWindowViewModel()
        {
            return new GameWindowViewModel(_randomizer, _players);
        }

        [Test]
        public void GameWindowViewModel_CanBeCreated()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
        }

        [Test]
        public void RollDiceCommand_InitialState_CanBeExecuted()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            ICommand command = viewModel.RollDiceCommand;
            command.Execute(null);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void RollDiceCommand_FirstPlayerStartsTheGameWithFirstRoll_ReturnsRollResults(
            int rollResult)
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            _randomizer.GetRandomNumber(1, 6).Returns(rollResult);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);

            Assert.AreEqual(rollResult, viewModel.RollResult[0]);
        }

        [Test]
        public void RollDiceCommand_PlayerRollsThreeTimes_CommandIsDisabled()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);
            command.Execute(null);
            command.Execute(null);
            var result = command.CanExecute(null);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void RollResult_PropertyChanged_IsFired()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            bool hasFired = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.RollResult))
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void UpdateTable_PropertyChanged_IsFired()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            bool hasFired = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.UpdateTable))
                    hasFired = true;
            };
            _randomizer.GetRandomNumber(1, 6).Returns(1);
            ICommand command = viewModel.RollDiceCommand;

            command.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void GameWindowViewModel_WhenCreated_PlayersReturnsGivenPlayersNames()
        {
            var expected = new[] { "A", "B", "C", "D" };
            GameWindowViewModel viewModel = new GameWindowViewModel(_randomizer, expected);
            var result = viewModel.Players;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PickCategoryCommand_CanBeExecuted()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            ICommand commandRoll = viewModel.RollDiceCommand;
            commandRoll.Execute(null);
            ICommand command = viewModel.PickCategoryCommand;
            command.Execute("Aces");
        }

        [Test]
        public void ActivePlayer_InitialState_ReturnsPlayerOneName()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();

            var result = viewModel.ActivePlayer;

            Assert.AreEqual("A's Turn:", result);
        }

        [Test]
        public void PickCategoryCommand_PlayerPickCategory_NextPlayerIsActive()
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            ICommand commandRoll = viewModel.RollDiceCommand;
            commandRoll.Execute(null);
            ICommand command = viewModel.PickCategoryCommand;
            command.Execute("Aces");
            var result = viewModel.ActivePlayer;

            Assert.AreEqual("B's Turn:", result);
        }

        [TestCase("Aces", 1, 5)]
        [TestCase("Twos", 2, 10)]
        public void PickCategoryCommand_PlayerPickCategory_GamestateIsUpdated(
            string choosenCategory, int rollResult, int expectedScore)
        {
            GameWindowViewModel viewModel = CreateGameWindowViewModel();
            _randomizer.GetRandomNumber(1, 6).Returns(rollResult);
            ICommand commandRoll = viewModel.RollDiceCommand;
            commandRoll.Execute(null);
            ICommand command = viewModel.PickCategoryCommand;
            command.Execute(choosenCategory);
            var parseCategory = (Category)Enum.Parse(typeof(Category), choosenCategory);
            var result = viewModel.UpdateTable[0][parseCategory];

            Assert.AreEqual(expectedScore, result);
        }
    }
}
