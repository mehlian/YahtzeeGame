namespace Yahtzee.Core
{
    public interface IRandomizer
    {
        int GetRandomNumber(int minValue, int maxValue);
    }
}