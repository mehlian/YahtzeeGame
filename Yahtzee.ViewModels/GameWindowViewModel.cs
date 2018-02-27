using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using RandomNumberGenerator;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged, IGameWindowRequestClose
    {
        private IRandomizer _randomizer;
        private IDice[] _dice;
        private Game _game;

        public GameWindowViewModel(IRandomizer randomizer, string[] players)
        {
            _randomizer = randomizer;
            _game = new Game(_randomizer);
            _game.NewGame(players);

            Players = new string[4];
            Players = players;
            ActivePlayer = Players[_game.ActivePlayer] + "'s Turn:";

            UpdateTable = new Dictionary<Category, int>[4];

            _dice = new[] {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            _rollDiceCommand =
                new CommandHandler((x) =>
                {
                    _game.RollDice(_dice);
                    RollResult = _game.RollResult.Select(y => y.Result).ToArray();
                    UpdateTable[_game.ActivePlayer] = _game.GetAvailableCategories();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));
                }, (x) => CanExecute());
        }

        private bool CanExecute()
        {
            if (_game.RollsLeft == 0)
                return false;
            return true;
        }

        private ICommand _rollDiceCommand;
        public ICommand RollDiceCommand
        {
            get
            {
                return _rollDiceCommand;
            }
        }

        private int[] _rollResult;
        public int[] RollResult
        {
            get
            {
                return _rollResult;
            }

            set
            {
                _rollResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RollResult)));
            }
        }

        private Dictionary<Category, int>[] _updateTable;
        public Dictionary<Category, int>[] UpdateTable
        {
            get
            {
                return _updateTable;
            }

            set
            {
                _updateTable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));
            }
        }

        public string[] Players { get; }
        public ICommand PickCategoryCommand
        {
            get
            {
                return new CommandHandler((x) =>
                {
                var parseCategory = (Category)Enum.Parse(typeof(Category), x.ToString());
                    _game.AddPoints(parseCategory);

                        //UpdateTable = _game.GameStatus();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));

                    ActivePlayer = Players?[_game.ActivePlayer] + "'s Turn:";

                }, (x) => true);
            }
        }

        public string ActivePlayer { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameWindowCloseRequestedEventArgs> CloseRequested;
    }
}