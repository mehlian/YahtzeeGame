namespace Yahtzee.Core
{
    public class Dice : IDice
    {
        public bool IsUnlocked { get; protected set; }
        public int Result { get; set; }
        public int SideNumber { get; }

        public Dice()
        {
            SideNumber = 6;
            IsUnlocked = true;
        }

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