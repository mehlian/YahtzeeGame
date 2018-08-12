using Yahtzee.Core;

namespace RandomNumberGenerator
{
    public class FakeRandomizer : IRandomizer
    {
        public int GetRandomNumber(int minValue, int maxValue)
        {
            return 1;
        }
    }
}
