using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Yahtzee.Core;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged, IGameWindowRequestClose
    {
        private IRandomizer _randomizer;
        private Game _game;

        public GameWindowViewModel(IRandomizer randomizer, params string[] players)
        {
            ResetGame(randomizer, players);

            ToggleDiceLockCommand = new DelegateCommand<object>((x) =>
            {
                if (IsPlayerAllowedToLockDice())
                    ToggleDiceLock(x);

                InvokeDiceProperty();
            });

            RollDiceCommand = new DelegateCommand(() =>
            {
                RollWithDiceSet();
                ShowRollResultsOnUI();

                if (IsLastRoll())
                    IsPlayerAllowedToRollDice = false;

            }).ObservesCanExecute(() => IsPlayerAllowedToRollDice);

            PickCategoryCommand = new DelegateCommand<object>((x) =>
            {
                AddPointsForActivePlayer(x);
                ScoreTable = UpdateScoreTable();
                IsPickCategoryCommandAvailable = DisablePickCategoryCommand();

                ActivePlayer = Players[_game.ActivePlayer];
                IsPlayerAllowedToRollDice = true;
                CalculateNewScore();
                UnlockAllDice();

                InvokeDiceProperty();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameWindowCloseRequestedEventArgs> CloseRequested;

        public ICommand RollDiceCommand { get; }
        public ICommand PickCategoryCommand { get; }
        public ICommand ToggleDiceLockCommand { get; }

        private IDice[] _dice;
        public IDice[] Dice
        {
            get { return _dice; }
            protected set
            {
                _dice = value;
                OnPropertyChanged();
            }
        }

        public string[] Players { get; protected set; }

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

        private ObservableCollection<Dictionary<Category, int?>> _scoreTable;
        public ObservableCollection<Dictionary<Category, int?>> ScoreTable
        {
            get { return _scoreTable; }
            protected set
            {
                _scoreTable = value;
                OnPropertyChanged();
            }
        }

        private bool _isPlayerAllowedToRollDice;
        private bool IsPlayerAllowedToRollDice
        {
            get { return _isPlayerAllowedToRollDice; }
            set
            {
                _isPlayerAllowedToRollDice = value;
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

        private void ResetGame(IRandomizer randomizer, string[] players)
        {
            _randomizer = randomizer;
            _game = new Game(_randomizer);
            _game.NewGame(players);
            _dice = MakeNewDiceSet();
            Players = players;
            ActivePlayer = Players[_game.ActivePlayer];
            ScoreTable = UpdateScoreTable();
            IsPickCategoryCommandAvailable = DisablePickCategoryCommand();
            IsPlayerAllowedToRollDice = true;
        }
        private void InvokeDiceProperty()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dice)));
        }
        private void InvokeIsPickCategoryCommandAvailableProperty()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPickCategoryCommandAvailable)));
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private IDice[] MakeNewDiceSet()
        {
            return new[] {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };
        }
        private void RollWithDiceSet()
        {
            _game.RollDice(_dice);
        }
        private void ToggleDiceLock(object x)
        {
            var dieNumber = int.Parse((string)x);
            if (Dice[dieNumber].IsUnlocked)
                Dice[dieNumber].Lock();
            else
                Dice[dieNumber].Unlock();
        }
        private void UnlockAllDice()
        {
            foreach (var dice in Dice)
            {
                dice.Unlock();
            }
        }
        private bool IsLastRoll()
        {
            return _game.RollsLeft == 0;
        }
        private bool IsPlayerAllowedToLockDice()
        {
            return _game.RollsLeft < 3;
        }
        private void ShowRollResultsOnUI()
        {
            Dice = _game.RollResult;
            ScoreTable[_game.ActivePlayer] = AvailableCategoriesProcessor();
        }
        private Dictionary<Category, int?> AvailableCategoriesProcessor()
        {
            var processor = new Dictionary<Category, int?>();

            foreach (var category in _game.GetAvailableCategories())
            {
                if (category.Value == null)
                    processor.Add(category.Key, _game.GameStatus()[_game.ActivePlayer][category.Key]);
                else
                {
                    processor.Add(category.Key, category.Value);
                    IsPickCategoryCommandAvailable[_game.ActivePlayer][category.Key] = true;
                }
            }
            InvokeIsPickCategoryCommandAvailableProperty();
            return processor;
        }
        private ObservableCollection<Dictionary<Category, bool>> DisablePickCategoryCommand()
        {
            var reset = new ObservableCollection<Dictionary<Category, bool>>();
            for (int i = 0; i < Players.Length; i++)
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
        private void AddPointsForActivePlayer(object x)
        {
            var parseCategory = (Category)Enum.Parse(typeof(Category), (string)x);
            _game.AddPoints(parseCategory);
        }
        private ObservableCollection<Dictionary<Category, int?>> UpdateScoreTable()
        {
            return new ObservableCollection<Dictionary<Category, int?>>(_game.GameStatus());
        }
        private void CalculateNewScore()
        {
            PartialScore = _game.PartialScore;
            BonusScore = _game.BonusScore;
            TotalScore = _game.TotalScore;
        }
    }
}