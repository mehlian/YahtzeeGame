using System;
using Yahtzee.Core;

namespace RandomNumberGenerator
{
    public class Randomizer : IRandomizer
    {
        private readonly Random rand = new Random();

        public int GetRandomNumber(int minNumber, int maxNumber)
        {
            return rand.Next(minNumber, maxNumber + 1);
        }
    }
}