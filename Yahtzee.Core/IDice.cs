namespace Yahtzee.Core
{
    public interface IDice
    {
        bool IsUnlocked { get; }
        int Result { get; set; }
        int SideNumber { get; }

        void Lock();
        void Unlock();
    }
}