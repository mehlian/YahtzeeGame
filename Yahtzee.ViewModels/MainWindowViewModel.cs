using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Yahtzee.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
        }
        private IGameWindowService _gameWindowService;
        public MainWindowViewModel(IGameWindowService gameWindowService)
        {
            _gameWindowService = gameWindowService;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NewGameCommand { get { return new CommandHandler((x) => { TabControlIndex = 1; }, (x) => true); } }

        private int _tabControlIndex;
        public int TabControlIndex
        {
            get { return _tabControlIndex; }
            set
            {
                _tabControlIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TabControlIndex)));
            }
        }

        public ICommand SelectNumberOfPlayersCommand
        {
            get
            {
                return new CommandHandler((x) =>
                {
                    NumberOfPlayers = int.Parse(x.ToString());
                    TabControlIndex = 2;
                }, (x) => true);
            }
        }

        public int NumberOfPlayers { get; set; } = 1;
        public ICommand BackCommand
        {
            get
            {
                return new CommandHandler((x) => { TabControlIndex = 0; }, (x) => true);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHandler((x) =>
                {
                    NumberOfPlayers = 1;
                    TabControlIndex = 1;
                    activePlayer = 1;
                    Message = $"Enter name for player {activePlayer}:";
                    PlayerName = $"Name{activePlayer}";
                }, (x) => true);
            }
        }

        private string _message = "Enter name for player 1:";
        public string Message { get { return _message; } set { _message = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message))); } }

        private int activePlayer = 1;
        public ICommand OKCommand
        {
            get
            {
                return new CommandHandler((x) =>
                {
                    if (activePlayer < NumberOfPlayers)
                    {
                        Players.Add(PlayerName);
                        activePlayer++;
                        PlayerName = $"Name{activePlayer}";
                        Message = $"Enter name for player {activePlayer}:";
                    }
                    else
                    {
                        ShowGameWindow();
                    }

                }, (x) => CanExecute(x));
            }
        }
        private string _playerName = $"Name1";
        public string PlayerName { get { return _playerName; } set { _playerName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerName))); } }
        public List<string> Players { get; set; } = new List<string>();

        private bool CanExecute(object param)
        {
            if (activePlayer > NumberOfPlayers)
            {
                return false;
            }
            return true;
        }

        // testing gamewindow
        public ICommand ShowGameWindowCommand
        {
            get
            {
                return new CommandHandler((x) =>
                {
                    ShowGameWindow();
                }, (x) => CanExecute(x));
            }
        }

        // testing gamewindow
        private void ShowGameWindow()
        {
            var viewModel = new GameWindowViewModel();
            bool? result = _gameWindowService.ShowDialog(viewModel);

            if (result.HasValue)
            {
                if (result.Value)
                {
                    //Accepted
                }
                else
                {
                    //Cancelled
                }
            }
        }
    }
}