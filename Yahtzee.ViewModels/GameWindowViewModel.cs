using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using NSubstitute;
using System.Linq;
using System.Collections.ObjectModel;
using RandomNumberGenerator;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged,IGameWindowRequestClose
    {
        private IRandomizer _randomizer;
        private IDice[] _dice;

        public GameWindowViewModel()
        {
            if (_randomizer == null)
            {
                //_randomizer = Substitute.For<IRandomizer>();
                //_randomizer.GetRandomNumber(1, 6).Returns(1, 1, 1, 1, 1, 6, 6, 3, 3, 3);
                _randomizer = new Randomizer();
            }
            UpdateTable = new Dictionary<Category, int>[4];

            _dice = new[] {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            _rollDiceCommand = new DelegateCommand(() =>
              {
                  Game game = new Game(_randomizer);
                  game.NewGame("Bob");

                  game.RollDice(_dice);
                  RollResult = game.RollResult.Select(x => x.Result).ToArray();
                  UpdateTable[0] = game.GetAvailableCategories();
                  game.RollDice(_dice);
                  //UpdateTable[1] = game.GetAvailableCategories();

                  //UpdateTable = UpdateTable;
                  //Category co = (Category)Enum.Parse(typeof(Category), "Aces");
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateTable)));

              });
        }

        public GameWindowViewModel(IRandomizer randomizer) : this()
        {
            _randomizer = randomizer;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameWindowCloseRequestedEventArgs> CloseRequested;
    }
}