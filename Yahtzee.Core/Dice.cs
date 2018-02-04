using System;

namespace Yahtzee.Core
{
    public class Dice
    {
        public bool IsUnlocked { get; protected set; } = true;
        public double Result { get; set; }

        public void Lock()
        {
            IsUnlocked = false;
        }

        public void Unlock()
        {
            IsUnlocked = true;
        }
    }
}