namespace Yahtzee.Core
{
    public interface IRandomizer
    {
        int Roll(int min, int max);
    }
}