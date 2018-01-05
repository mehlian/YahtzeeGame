namespace Yahtzee.Core
{
    public interface IDice
    {
        DiceStatus Status { get; set; }
        int Result { get; set; }

        int Roll();
    }
}