using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee.Core
{
    internal class RegularScorer :Scorer
    {
        private int[] _rollResult;

        internal override int CalculateCategoryScore(Category category, int[] rollResult)
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

        internal override int? CalculateBonusScore(Dictionary<Category, int?> gameStatus)
        {
            if (gameStatus.Take(6).All(x => x.Value != null) && gameStatus.Take(6).Sum(x => x.Value) < 63)
                return 0;
            if (gameStatus.Take(6).Any(x => x.Value == null))
                return null;
            else
                return 35;
        }

        internal override int? CalculatePartialScore(Dictionary<Category, int?> gameStatus)
        {
            var bonusScore = CalculateBonusScore(gameStatus) == null ? 0 : (int)CalculateBonusScore(gameStatus);
            return gameStatus.Take(6).Any(x => x.Value == null) ? null : gameStatus.Take(6).Sum(x => x.Value) + bonusScore;
        }

        internal override int? CalculateTotalScore(Dictionary<Category, int?> gameStatus)
        {
            var bonusScore = CalculateBonusScore(gameStatus) == null ? 0 : (int)CalculateBonusScore(gameStatus);
            if (gameStatus.Any(x => x.Value == null))
                return null;
            else
                return (int)gameStatus.Sum(x => x.Value) + bonusScore;
        }

        private int CalculateScoreForGivenSideNumber(int side)
        {
            return _rollResult.Where(x => x == side).Sum();
        }

        private int CalculateScoreForThreeOfKind()
        {
            return _rollResult.GroupBy(x => x).Any(x => x.Count() >= 3) ? _rollResult.Sum() : 0;
        }

        private int CalculateScoreForFourOfKind()
        {
            return _rollResult.GroupBy(x => x).Any(x => x.Count() >= 4) ? _rollResult.Sum() : 0;
        }

        private int CalculateScoreForFullHouse()
        {
            return _rollResult.GroupBy(x => x).Count() == 2 && _rollResult.GroupBy(x => x).Any(x => x.Count() == 2) ? 25 : 0;
        }

        private int CalculateScoreForSmallStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = _rollResult.Distinct().OrderBy(x => x);
            return sub.Count() > 3 && master.SkipWhile((x, i) => !master.Skip(i).Take(4).SequenceEqual(sub.Take(4)))
                        .Take(4).DefaultIfEmpty().SequenceEqual(sub.Take(4)) ? 30 : 0;
        }

        private int CalculateScoreForLargeStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = _rollResult.Distinct().OrderBy(x => x);
            return sub.Count() == 5 && master.SkipWhile((x, i) => !master.Skip(i).Take(5).SequenceEqual(sub))
                        .Take(5).DefaultIfEmpty().SequenceEqual(sub) ? 40 : 0;
        }

        private int CalculateScoreForChance()
        {
            return _rollResult.Sum();
        }

        private int CalculateScoreForYahtzee()
        {
            return _rollResult.GroupBy(x => x).Count() == 1 ? 50 : 0;
        }
    }
}
