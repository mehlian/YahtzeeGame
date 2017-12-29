using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Core
{
    public class Dice
    {
        private readonly IRandomizer _randomizer;

        public DiceStatus Status { get; set; } = DiceStatus.UnLocked;
        public int Result { get; set; }

        public Dice(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public int Roll()
        {
            Result = _randomizer.GetRandomInt();
            return Result;
        }
    }

}
