namespace Yahtzee.Core
{
    public interface IRandomizer
    {
        int GetRandomInt(int minNumber, int maxNumber);
    }
}