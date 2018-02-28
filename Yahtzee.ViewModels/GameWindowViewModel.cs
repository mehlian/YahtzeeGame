using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using RandomNumberGenerator;
using System.Runtime.CompilerServices;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged, IGameWindowRequestClose
    {
        private IRandomizer _randomizer;
        private IDice[] _dice;
        private Game _game;

        public GameWindowViewModel(IRandomizer randomizer, params string[] players)
        {
            _randomizer = randomizer;
            _game = new Game(_randomizer);
            _game.NewGame(players);

            Players = new string[4];
            Players = players;
            ActivePlayer = Players[_game.ActivePlayer] + "'s Turn:";
            PartialScore = new int?[4];
            BonusScore = new int?[4];
            TotalScore = new int?[4];

            UpdateTable = new Dictionary<Category, int>[4];

            _dice = new[] {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            RollDiceCommand = new DelegateCommand(() =>
                                {
                                    _game.RollDice(_dice);
                                    RollResult = _game.RollResult.Select(y => y.Result).ToArray();
                                    UpdateTable[_game.ActivePlayer] = _game.GetAvailableCategories();
                                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));
                                    if (_game.RollsLeft == 0)
                                    {
                                        RollsLeft = false;
                                    }
                                }).ObservesCanExecute(() => RollsLeft);

            PickCategoryCommand = new DelegateCommand<object>((x) =>
                                {
                                    var parseCategory = (Category)Enum.Parse(typeof(Category), x.ToString());
                                    _game.AddPoints(parseCategory);

                                    //UpdateTable = _game.GameStatus();

                                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));

                                    ActivePlayer = Players?[_game.ActivePlayer] + "'s Turn:";
                                    RollsLeft = true;
                                    PartialScore = _game.PartialScore;
                                    BonusScore = _game.BonusScore;
                                    TotalScore = _game.TotalScore;
                                });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameWindowCloseRequestedEventArgs> CloseRequested;

        public ICommand RollDiceCommand { get; }
        public ICommand PickCategoryCommand { get; }

        public string[] Players { get; }
        private string _activePlayer;
        public string ActivePlayer
        {
            get { return _activePlayer; }
            protected set
            {
                _activePlayer = value;
                OnPropertyChanged();
            }
        }

        private int[] _rollResult;
        public int[] RollResult
        {
            get { return _rollResult; }
            set
            {
                _rollResult = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<Category, int>[] _updateTable;
        public Dictionary<Category, int>[] UpdateTable
        {
            get { return _updateTable; }
            set
            {
                _updateTable = value;
                OnPropertyChanged();
            }
        }

        private bool _rollsLeft = true;
        private bool RollsLeft
        {
            get { return _rollsLeft; }
            set
            {
                _rollsLeft = value;
                OnPropertyChanged();
            }
        }

        private int?[] _partialScore;
        public int?[] PartialScore
        {
            get { return _partialScore; }
            protected set
            {
                _partialScore = value;
                OnPropertyChanged();
            }
        }

        private int?[] _bonusScore;
        public int?[] BonusScore
        {
            get { return _bonusScore; }
            protected set
            {
                _bonusScore = value;
                OnPropertyChanged();
            }
        }

        private int?[] _totalScore;
        public int?[] TotalScore
        {
            get { return _totalScore; }
            protected set
            {
                _totalScore = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}