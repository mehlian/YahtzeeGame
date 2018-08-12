namespace Yahtzee.Core
{
    internal static class ScorerFactory
    {
        internal static Scorer GetScorer(bool hasYahtzee)
        {
            if (hasYahtzee)
                return new YahtzeeScorer();
            else
                return new RegularScorer();
        }
    }
}
