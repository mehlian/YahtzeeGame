using System;
using System.Collections.Generic;

namespace Yahtzee.Core
{
    public class Game
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 6;
        private IRandomizer _randomizer;

        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public List<Dice> RollDice(List<Dice> dice)
        {
            foreach (Dice die in dice)
            {
                if (die.IsUnlocked)
                {
                    die.Result = _randomizer.Roll(MIN_VALUE, MAX_VALUE);
                }
            }

            return dice;
        }
    }
}