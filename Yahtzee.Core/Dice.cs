namespace Yahtzee.Core
{
    public class Dice : MustInitialize<IRandomizer>, IDice
    {
        private const int MIN_NUMBER = 1;
        private const int MAX_NUMBER = 6;
        private readonly IRandomizer _randomizer;
        public DiceStatus Status { get; set; } = DiceStatus.UnLocked;
        public int Result { get; set; }

        public Dice(IRandomizer parameters) : base(parameters)
        {
            _randomizer = parameters;
        }

        public int Roll()
        {
            Result = _randomizer.GetRandomInt(MIN_NUMBER, MAX_NUMBER);
            return Result;
        }
    }

}
