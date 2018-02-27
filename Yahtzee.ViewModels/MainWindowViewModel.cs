using Prism.Commands;
using RandomNumberGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Yahtzee.Core;

namespace Yahtzee.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IGameWindowService _gameWindowService;
        private int _tabControlIndex;
        private string _message;
        private int _defaultNameIndex;
        private string _playerName;

        public MainWindowViewModel(IGameWindowService gameWindowService)
        {
            _gameWindowService = gameWindowService;

            Players = new List<string>();

            NewGameCommand = new DelegateCommand(() => TabControlIndex = 1);
            SelectNumberOfPlayersCommand = new DelegateCommand<object>((x) =>
                                                {
                                                    SetDefaultValues();
                                                    NumberOfPlayers = int.Parse(x.ToString());
                                                    TabControlIndex = 2;
                                                });
            BackCommand = new DelegateCommand(() => { TabControlIndex = 0; });
            OKCommand = new DelegateCommand(() =>
                                                {
                                                    if (Players.Count < NumberOfPlayers)
                                                    {
                                                        Players.Add(PlayerName);
                                                    }
                                                    if (Players.Count < NumberOfPlayers)
                                                    {
                                                        _defaultNameIndex++;
                                                        PlayerName = $"Name{_defaultNameIndex}";
                                                        Message = $"Enter name for player {_defaultNameIndex}:";
                                                    }
                                                    if (Players.Count == NumberOfPlayers)
                                                    {
                                                        TabControlIndex = 0;
                                                        ShowGameWindow();
                                                    }

                                                });
            CancelCommand = new DelegateCommand(() =>
                                                {
                                                    SetDefaultValues();
                                                    TabControlIndex = 1;
                                                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NewGameCommand { get; }
        public ICommand SelectNumberOfPlayersCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand OKCommand { get; }
        public ICommand CancelCommand { get; }

        public int NumberOfPlayers { get; protected set; }

        public int TabControlIndex
        {
            get { return _tabControlIndex; }
            set
            {
                _tabControlIndex = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            protected set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        public List<string> Players { get; protected set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetDefaultValues()
        {
            NumberOfPlayers = 1;
            _defaultNameIndex = 1;
            Players.Clear();
            Message = $"Enter name for player {_defaultNameIndex}:";
            PlayerName = $"Name{_defaultNameIndex}";
        }

        // testing gamewindow
        public virtual void ShowGameWindow()
        {
            var players = Players.ToArray();
            SetDefaultValues();
            IRandomizer randomizer = new Randomizer();
            var viewModel = new GameWindowViewModel(randomizer, players);
            bool? result = _gameWindowService.ShowDialog(viewModel);

            //if (result.HasValue)
            //{
            //    if (result.Value)
            //    {
            //        //Accepted
            //    }
            //    else
            //    {
            //        //Cancelled
            //    }
            //}
        }
    }
}