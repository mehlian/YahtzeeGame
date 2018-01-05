namespace Yahtzee.Core
{
    public interface ITable
    {
        int Aces { get; set; }
        int Twos { get; set; }
        int Threes { get; set; }
        int Fours { get; set; }
        int Fives { get; set; }
        int Sixes { get; set; }

        int ThreeOfAKind { get; set; }
        int FourOfAKind { get; set; }
        int FullHouse { get; set; }
        int SmallStraight { get; set; }
        int LargeStraight { get; set; }
        int Yahtzee { get; set; }
        int Chance { get; set; }
    }
}