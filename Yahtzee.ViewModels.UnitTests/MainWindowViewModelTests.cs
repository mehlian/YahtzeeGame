using NSubstitute;
using NUnit.Framework;
using System.Windows.Input;

namespace Yahtzee.ViewModels.UnitTests
{
    [TestFixture]
    public class MainWindowViewModelTests
    {
        private IGameWindowService _gameWindowService;
        private MainWindowViewModel _vm;

        [SetUp]
        public void Setup()
        {
            _gameWindowService = Substitute.For<IGameWindowService>();
            _vm = Substitute.ForPartsOf<MainWindowViewModel>(_gameWindowService);
            _vm.When(x => x.ShowGameWindow()).DoNotCallBase();
        }

        [Test]
        public void MainWindowViewModel_CanBeCreated()
        {
            MainWindowViewModel vm = new MainWindowViewModel(_gameWindowService);
        }

        [Test]
        public void TabControlIndex_InitialState_Returns0()
        {
            var result = _vm.TabControlIndex;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void NewGameCommand_CanBeExecuted()
        {
            ICommand newGameCommand = _vm.NewGameCommand;

            newGameCommand.Execute(null);
        }

        [Test]
        public void NewGameCommand_CommandExecuted_TabControlIndexReturns1()
        {
            ICommand newGameCommand = _vm.NewGameCommand;

            newGameCommand.Execute(null);
            var result = _vm.TabControlIndex;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TabControlIndex_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.TabControlIndex))
                    hasFired = true;
            };
            ICommand newGameCommand = _vm.NewGameCommand;

            newGameCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_CanBeExecuted()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;

            selectNumberOfPlayersCommand.Execute(0);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_CommandExecuted_NumberOfPlayersReturnsGivenNumber()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;

            selectNumberOfPlayersCommand.Execute(1);
            var result = _vm.NumberOfPlayers;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_CommandExecuted_TabControlIndexReturns2()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;

            selectNumberOfPlayersCommand.Execute(1);
            var result = _vm.TabControlIndex;

            Assert.AreEqual(2, result);
        }

        //[Test]
        //public void SelectNumberOfPlayersCommand_ButtonWithNumberIsPressed_MessageReturnsString()
        //{
        //    MainWindowViewModel vm = CreateMainWindowViewModel();

        //    ICommand command = vm.SelectNumberOfPlayersCommand;
        //    command.Execute(1);
        //    var result = vm.Message;

        //    StringAssert.Contains("Enter name for player 1:", result);
        //}

        [Test]
        public void BackCommand_CanBeExecuted()
        {
            ICommand backCommand = _vm.BackCommand;

            backCommand.Execute(null);
        }

        [Test]
        public void BackCommand_CommandExecuted_TabControlIndexReturns0()
        {
            ICommand backCommand = _vm.BackCommand;
            ICommand newGameCommand = _vm.NewGameCommand;

            newGameCommand.Execute(null);
            backCommand.Execute(null);
            var result = _vm.TabControlIndex;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void PlayerName_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.PlayerName))
                    hasFired = true;
            };

            _vm.PlayerName = "A";

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void OKCommand_CanBeExecuted()
        {
            ICommand okCommand = _vm.OKCommand;

            okCommand.Execute(null);
        }

        [Test]
        public void OKCommand_CommandExecutedWithGivenName_NameIsStored()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(1);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            var result = _vm.Players[0];

            Assert.AreEqual("A", result);
        }

        [Test]
        public void OKCommand_CommandExecutedWithGivenNameForFirstPlayer_PlayerNameReturnsDefaultNameForNextPlayer()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            var result = _vm.PlayerName;

            Assert.AreEqual("Name2", result);
        }

        [Test]
        public void OKCommand_CommandExecutedWithGivenNameForFirstPlayer_MessageIsUpdatedForNextPlayer()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            var result = _vm.Message;

            Assert.AreEqual("Enter name for player 2:", result);
        }

        [Test]
        public void Message_PropertyChanged_IsFired()
        {
            bool hasFired = false;
            _vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_vm.Message))
                    hasFired = true;
            };
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void OKCommand_WhenAllPlayersNamesAreGiven_MessageIsNotUpdated()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            var expected = _vm.Message;
            _vm.PlayerName = "B";
            okCommand.Execute(null);
            var result = _vm.Message;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void OKCommand_WhenAllPlayersNamesAreGiven_PlayerNameIsNotUpdated()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            _vm.PlayerName = "B";
            var expected = _vm.PlayerName;
            okCommand.Execute(null);
            var result = _vm.PlayerName;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void OKCommand_GivenExtraNameWhenAllNamesAreStored_ExtraNameIsNotStored()
        {

            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            _vm.PlayerName = "B";
            okCommand.Execute(null);
            _vm.PlayerName = "C";
            okCommand.Execute(null);

            var result = _vm.Players.Count;

            Assert.AreEqual(2, result);
        }

        [Test]
        public void OKCommand_GivenExtraNameWhenAllNamesAreStored_ShowGameWindowIsInvoked()
        {
            bool isFired = false;
            _vm.When(x => x.ShowGameWindow()).Do(x => isFired = true);
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;

            selectNumberOfPlayersCommand.Execute(2);
            _vm.PlayerName = "A";
            okCommand.Execute(null);
            _vm.PlayerName = "B";
            okCommand.Execute(null);

            Assert.IsTrue(isFired);
        }

        [Test]
        public void CancelCommand_CanBeExecuted()
        {
            ICommand cancelCommand = _vm.CancelCommand;

            cancelCommand.Execute(null);
        }

        [Test]
        public void CancelCommand_CommandExecuted_TabControlIndexReturns1()
        {
            ICommand cancelCommand = _vm.CancelCommand;

            cancelCommand.Execute(null);
            var result = _vm.TabControlIndex;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void CancelCommand_CommandExecuted_PlayersCountReturns0()
        {
            ICommand cancelCommand = _vm.CancelCommand;

            _vm.Players.Add("A");
            cancelCommand.Execute(null);
            var result = _vm.Players.Count;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void CancelCommand_CommandExecuted_NumberOfPlayersReturns1()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand cancelCommand = _vm.CancelCommand;

            selectNumberOfPlayersCommand.Execute(2);
            cancelCommand.Execute(null);
            var result = _vm.NumberOfPlayers;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void CancelCommand_CommandExecuted_PlayerNameReturnsDefaultName()
        {
            ICommand selectNumberOfPlayersCommand = _vm.SelectNumberOfPlayersCommand;
            ICommand okCommand = _vm.OKCommand;
            ICommand cancelCommand = _vm.CancelCommand;

            selectNumberOfPlayersCommand.Execute(2);
            okCommand.Execute(null);
            cancelCommand.Execute(null);
            var result = _vm.PlayerName;

            Assert.AreEqual("Name1", result);
        }
    }
}
