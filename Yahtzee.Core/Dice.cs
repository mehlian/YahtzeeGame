using System;

namespace Yahtzee.Core
{
    public class Dice
    {
        private int result;

        public int Result
        {
            get
            {
                return result;
            }
            set
            {
                if (!IsLocked)
                    result = value;
            }
        }
        public bool IsLocked { get; set; }
    }
}