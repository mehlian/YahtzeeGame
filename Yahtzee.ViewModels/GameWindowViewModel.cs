using System;
using Prism.Commands;
using System.Windows.Input;
using Yahtzee.Core;
using System.Collections.Generic;
using System.ComponentModel;
using Yahtzee.CoreModule;
using Yahtzee.Randomizer;
using NSubstitute;

namespace Yahtzee.ViewModels
{
    public class GameWindowViewModel : INotifyPropertyChanged
    {
        private IRandomizer _randomizer;
        private List<Dice> _dice;

        public GameWindowViewModel()
        {
            if (_randomizer == null)
            {
                _randomizer = Substitute.For<IRandomizer>();
                _randomizer.Roll(1, 6).Returns(-1);
                _randomizer = new Randomizer.Randomizer();

            }

            //_randomizer = randomizer;
            //_randomizer = new BindingModule().GetRandomizer;
            _dice = new List<Dice>
            {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };

            _rollDiceCommand = new DelegateCommand(() =>
              {
                  //IRandomizer _randomizer = new Randomizer.Randomizer();
                  //IRandomizer _randomizer = Substitute.For<IRandomizer>();
                  Game game = new Game(_randomizer);
                  game.RollDice(_dice);
                  Die1 = game.RollResult[0].Result;
              });
        }

        public GameWindowViewModel(IRandomizer randomizer) : this()
        {

            _randomizer = randomizer;
            //throw new Exception($"Uh? {_randomizer.Roll(1,6)}");
        }

        private ICommand _rollDiceCommand;
        public ICommand RollDiceCommand
        {
            get
            {
                return _rollDiceCommand;
            }
        }

        private double _die1;
        public double Die1
        {
            get
            {
                return _die1;
            }

            set
            {
                _die1 = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Die1)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}