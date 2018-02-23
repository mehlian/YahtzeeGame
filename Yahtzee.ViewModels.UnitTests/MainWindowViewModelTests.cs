using NUnit.Framework;
using System.Windows.Input;

namespace Yahtzee.ViewModels.UnitTests
{
    [TestFixture]
    public class MainWindowViewModelTests
    {
        [Test]
        public void MainWindowViewModel_CanBeCreated()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
        }

        [Test]
        public void NewGameCommand_CanBeExecuted()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand command = vm.NewGameCommand;
            command.Execute(null);
        }

        [Test]
        public void TabControlIndex_InitialState_Returns0()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            var result = vm.TabControlIndex;

            Assert.AreEqual(0, result);
        }

        [Test]
        public void NewGameCommand_NewGameButtonIsPressed_TabControlIndexReturns1()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.NewGameCommand;
            command.Execute(null);
            var result = vm.TabControlIndex;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TabControlIndex_PropertyChangedOnNewGameCommand_IsFired()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            bool hasFired = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(vm.TabControlIndex))
                    hasFired = true;
            };
            ICommand command = vm.NewGameCommand;
            command.Execute(null);

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_CanBeExecuted()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand command = vm.SelectNumberOfPlayersCommand;
            command.Execute(1);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_ButtonWithNumberIsPressed_NumbersOfPlayersReturnsSelectedNumber()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.SelectNumberOfPlayersCommand;
            command.Execute(1);
            var result = vm.NumberOfPlayers;

            Assert.AreEqual(1, result);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_ButtonWithNumberIsPressed_TabControlIndexReturns2()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.SelectNumberOfPlayersCommand;
            command.Execute(1);
            var result = vm.TabControlIndex;

            Assert.AreEqual(2, result);
        }

        [Test]
        public void SelectNumberOfPlayersCommand_ButtonWithNumberIsPressed_MessageReturnsString()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.SelectNumberOfPlayersCommand;
            command.Execute(1);
            var result = vm.Message;

            StringAssert.Contains("Enter name for player 1:", result);
        }

        [Test]
        public void BackCommand_CanBeExecuted()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.BackCommand;
            command.Execute(null);
        }

        [Test]
        public void BackCommand_WhenClicked_TabControlIndexReturns0()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.BackCommand;
            ICommand commandNewGame = vm.NewGameCommand;
            commandNewGame.Execute(null);
            command.Execute(null);

            Assert.AreEqual(0, vm.TabControlIndex);
        }

        [Test]
        public void CancelCommand_CanBeExecuted()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.CancelCommand;
            command.Execute(null);
        }

        [Test]
        public void CancelCommand_WhenClicked_TabControlIndexReturns1()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

            ICommand command = vm.CancelCommand;
            command.Execute(null);

            Assert.AreEqual(1, vm.TabControlIndex);
        }

        [Test]
        public void CancelCommand_WhenClicked_NumberOfPlayersReturns1()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            commandPlNb.Execute(3);
            ICommand command = vm.CancelCommand;
            command.Execute(null);

            Assert.AreEqual(1, vm.NumberOfPlayers);
        }

        [Test]
        public void CancelCommand_WhenClicked_PlayerNameReturnsName1()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            commandPlNb.Execute(3);
            ICommand commandOK = vm.OKCommand;
            commandOK.Execute(null);
            ICommand command = vm.CancelCommand;
            command.Execute(null);

            Assert.AreEqual("Name1", vm.PlayerName);
        }

        [Test]
        public void PlayerName_InitialValue_ReturnsPlayerOne()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            var result=vm.PlayerName;

            Assert.AreEqual("Name1", result);
        }

        [Test]
        public void PlayerName_PropertyChanged_IsFired()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            bool hasFired = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(vm.PlayerName))
                    hasFired = true;
            };
            vm.PlayerName = "A";

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void OKCommand_CanBeExecuted()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand command = vm.OKCommand;
            command.Execute(null);
        }

        [Test]
        public void OKCommand_EnteredValidName_NameIsRegistered()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            commandPlNb.Execute(2);
            vm.PlayerName = "A";
            ICommand commandOK = vm.OKCommand;
            commandOK.Execute(null);
            var result = vm.Players;

            Assert.AreEqual("A", result[0]);
        }

        [Test]
        public void OKCommand_EnteredValidName_PlayerNameReturnsName()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            commandPlNb.Execute(2);
            vm.PlayerName = "A";
            ICommand commandOK = vm.OKCommand;
            commandOK.Execute(null);
            var result = vm.PlayerName;

            Assert.AreEqual("Name2", result);
        }

        [Test]
        public void OKCommand_GivenValidName_MessageIsChanged()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            commandPlNb.Execute(2);
            vm.PlayerName = "A";
            ICommand commandOK = vm.OKCommand;
            commandOK.Execute(null);
            var result = vm.Message;

            Assert.AreEqual("Enter name for player 2:", result);
        }

        [Test]
        public void Message_PropertyChanged_IsFired()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            bool hasFired = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(vm.Message))
                    hasFired = true;
            };
            vm.Message = "A";

            Assert.IsTrue(hasFired);
        }

        [Test]
        public void NumberOfPlayers_PropertyChanged_IsFired()
        {
            MainWindowViewModel vm = new MainWindowViewModel();

        }

        [Test]
        public void OKCommand_WhenAllPlayersNamesAreGiven_CantExecuteOKCommand()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            ICommand commandPlNb = vm.SelectNumberOfPlayersCommand;
            ICommand commandOK = vm.OKCommand;
            commandPlNb.Execute(2);
            vm.PlayerName = "A";
            commandOK.Execute(null);
            vm.PlayerName = "B";
            commandOK.Execute(null);

            var result = commandOK.CanExecute(null);

            Assert.AreEqual(false, result);
        }
    }
}
