namespace Yahtzee.Core
{
    public interface IRandomizer
    {
        int Roll(int minValue, int maxValue);
    }
}