using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
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

            UpdateTable = new ObservableCollection<Dictionary<Category, int?>>(_game.GameStatus());

            IsPickCategoryCommandAvailable = DisablePickCategoryCommand();

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

                UpdateTable[_game.ActivePlayer] = AvailableCategoriesProcessor();

                if (_game.RollsLeft == 0)
                {
                    RollsLeft = false;
                }
            }).ObservesCanExecute(() => RollsLeft);

            PickCategoryCommand = new DelegateCommand<object>((x) =>
            {
                var parseCategory = (Category)Enum.Parse(typeof(Category), x.ToString());
                _game.AddPoints(parseCategory);

                UpdateTable = new ObservableCollection<Dictionary<Category, int?>>(_game.GameStatus());
                IsPickCategoryCommandAvailable = DisablePickCategoryCommand();

                ActivePlayer = Players?[_game.ActivePlayer] + "'s Turn:";
                RollsLeft = true;
                PartialScore = _game.PartialScore;
                BonusScore = _game.BonusScore;
                TotalScore = _game.TotalScore;
            });
        }

        private ObservableCollection<Dictionary<Category,bool>> DisablePickCategoryCommand()
        {
            var reset = new ObservableCollection<Dictionary<Category, bool>>();
            for (int i = 0; i < 4; i++)
            {
                reset.Add(new Dictionary<Category, bool>
                {
                    { Category.Aces, false },
                    { Category.Twos, false },
                    { Category.Threes, false },
                    { Category.Fours, false },
                    { Category.Fives, false },
                    { Category.Sixes, false },
                    { Category.ThreeOfKind, false },
                    { Category.FourOfKind, false },
                    { Category.FullHouse, false },
                    { Category.SmallStraight, false },
                    { Category.LargeStraight, false },
                    { Category.Chance, false },
                    { Category.Yahtzee, false },
                });
            }
            return reset;
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

        private ObservableCollection<Dictionary<Category, int?>> _updateTable;
        public ObservableCollection<Dictionary<Category, int?>> UpdateTable
        {
            get { return _updateTable; }
            protected set
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

        private ObservableCollection<Dictionary<Category, bool>> isPickCategoryCommandAvailable;
        public ObservableCollection<Dictionary<Category, bool>> IsPickCategoryCommandAvailable
        {
            get { return isPickCategoryCommandAvailable; }
            protected set
            {
                isPickCategoryCommandAvailable = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<Category, int?> AvailableCategoriesProcessor()
        {
            Dictionary<Category, int?> processor = new Dictionary<Category, int?>();
            Dictionary<Category, bool> isAvailable = new Dictionary<Category, bool>();

            foreach (var category in _game.GetAvailableCategories())
            {
                if (category.Value == null)
                {
                    processor.Add(category.Key, _game.GameStatus()[_game.ActivePlayer][category.Key]);
                    isAvailable.Add(category.Key, false);
                }
                else
                {
                    processor.Add(category.Key, category.Value);
                    isAvailable.Add(category.Key, true);
                }
            }

            IsPickCategoryCommandAvailable[_game.ActivePlayer] = isAvailable;
            return processor;
        }
    }
}