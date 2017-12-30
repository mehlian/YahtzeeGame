using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Core
{
    public class Randomizer : IRandomizer
    {
        private readonly Random rand = new Random();

        public int GetRandomInt(int minNumber, int maxNumber)
        {
            return rand.Next(minNumber, maxNumber + 1);
        }
    }
}
