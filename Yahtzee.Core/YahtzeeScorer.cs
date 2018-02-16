using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Core
{
    public class YahtzeeScorer
    {
        private IDice[] _rollResult;

        public int CalculateScore(Category category, IDice[] rollResult)
        {
            _rollResult = rollResult;

            switch (category)
            {
                case Category.Aces:
                    return CalculateScoreForGivenSideNumber(1);
                case Category.Twos:
                    return CalculateScoreForGivenSideNumber(2);
                case Category.Threes:
                    return CalculateScoreForGivenSideNumber(3);
                case Category.Fours:
                    return CalculateScoreForGivenSideNumber(4);
                case Category.Fives:
                    return CalculateScoreForGivenSideNumber(5);
                case Category.Sixes:
                    return CalculateScoreForGivenSideNumber(6);
                case Category.ThreeOfKind:
                    return CalculateScoreForThreeOfKind();
                case Category.FourOfKind:
                    return CalculateScoreForFourOfKind();
                case Category.FullHouse:
                    return CalculateScoreForFullHouse();
                case Category.SmallStraight:
                    return CalculateScoreForSmallStraight();
                case Category.LargeStraight:
                    return CalculateScoreForLargeStraight();
                case Category.Chance:
                    return CalculateScoreForChance();
                case Category.Yahtzee:
                    return CalculateScoreForYahtzee();
                default:
                    throw new ArgumentException($"Unsupported category {category}.");
            }
        }

        private int CalculateScoreForYahtzee()
        {
            return _rollResult.GroupBy(x => x.Result).Count() == 1 ? 50 : 0;
        }

        private int CalculateScoreForChance()
        {
            return _rollResult.Sum(x => x.Result);
        }

        private int CalculateScoreForLargeStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = _rollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            return sub.Length == 5 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                        .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 40 : 0;
        }

        private int CalculateScoreForSmallStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = _rollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            return sub.Length > 3 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                        .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 30 : 0;
        }

        private int CalculateScoreForFullHouse()
        {
            return _rollResult.GroupBy(x => x.Result).Count() == 2 ? 25 : 0;
        }

        private int CalculateScoreForFourOfKind()
        {
            return _rollResult.GroupBy(x => x.Result).Any(x => x.Count() >= 4) ? _rollResult.Sum(x => x.Result) : 0;
        }

        private int CalculateScoreForThreeOfKind()
        {
            return _rollResult.GroupBy(x => x.Result).Any(x => x.Count() >= 3) ? _rollResult.Sum(x => x.Result) : 0;
        }

        private int CalculateScoreForGivenSideNumber(int side)
        {
            return _rollResult.Where(x => x.Result == side).Sum(x => x.Result);
        }
    }
}
