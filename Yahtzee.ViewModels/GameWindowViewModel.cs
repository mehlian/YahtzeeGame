using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using NSubstitute;
using System.Linq;
using System.Collections.ObjectModel;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged
    {
        private IRandomizer _randomizer;
        private IDice[] _dice;

        public GameWindowViewModel()
        {
            if (_randomizer == null)
            {
                _randomizer = Substitute.For<IRandomizer>();
                _randomizer.Roll(1, 6).Returns(1, 1, 1, 1, 1, 6, 6, 3, 3, 3);
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
                  game.NewGame(new[] { "Bob" });

                  game.RollDice(_dice);
                  RollResult = game.RollResult.Select(x => x.Result).ToArray();
                  UpdateTable[0] = game.GetAvailableOptions();
                  game.RollDice(_dice);
                  UpdateTable[1] = game.GetAvailableOptions();

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
    }
}