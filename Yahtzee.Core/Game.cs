using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee.Core
{
    public class Game
    {
        private IRandomizer _randomizer;
        private Dictionary<Category, int?>[] _gameStatus;

        public string[] Players { get; protected set; }
        public int ActivePlayer { get; protected set; }
        public IDice[] RollResult { get; protected set; }
        public int RollsLeft { get; protected set; } = 0;
        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public void NewGame(params string[] playerName)
        {
            if (playerName.Length > 4)
                throw new ArgumentException("Max number of players is 4.");

            Players = playerName;
            ActivePlayer = 0;
            RollsLeft = 3;

            _gameStatus = new Dictionary<Category, int?>[playerName.Length];
            for (int i = 0; i < playerName.Length; i++)
            {
                _gameStatus[i] = new Dictionary<Category, int?>
                {
                    { Category.Aces, null },
                    { Category.Twos, null },
                    { Category.Threes, null },
                    { Category.Fours, null },
                    { Category.Fives, null },
                    { Category.Sixes, null },
                    { Category.ThreeOfKind, null },
                    { Category.FourOfKind, null },
                    { Category.FullHouse, null },
                    { Category.SmallStraight, null },
                    { Category.LargeStraight, null },
                    { Category.Chance, null },
                    { Category.Yahtzee, null },
                };
            }
        }

        public void RollDice(IDice[] dice)
        {
            if (RollsLeft < 1)
                throw new InvalidOperationException($"Player {Players[ActivePlayer]} has exceeded the maximum number of dice rolls in this turn.");

            if (dice.Length != 5)
                throw new ArgumentException("Only five dice are supported.");

            foreach (var die in dice)
            {
                if (die.IsUnlocked)
                {
                    die.Result = _randomizer.Roll(1, die.SideNumber);
                }
            }

            RollResult = dice;
            RollsLeft--;
        }

        public Dictionary<Category, int> GetAvailableOptions(string playerName)
        {
            int acesScore = (int)RollResult.Where(x => x.Result == 1).Sum(x => x.Result);
            int twosScore = (int)RollResult.Where(x => x.Result == 2).Sum(x => x.Result);
            int threesScore = (int)RollResult.Where(x => x.Result == 3).Sum(x => x.Result);
            int foursScore = (int)RollResult.Where(x => x.Result == 4).Sum(x => x.Result);
            int fivesScore = (int)RollResult.Where(x => x.Result == 5).Sum(x => x.Result);
            int sixesScore = (int)RollResult.Where(x => x.Result == 6).Sum(x => x.Result);

            int threeOfKindScore = RollResult.GroupBy(x => x.Result).Where(x => x.Count() == 3) != null ? (int)RollResult.Sum(x => x.Result) : 0;
            int fourOfKindScore = RollResult.GroupBy(x => x.Result).Where(x => x.Count() == 4) != null ? (int)RollResult.Sum(x => x.Result) : 0;
            int fullHouseScore = RollResult.GroupBy(x => x.Result).Count() == 2 ? 25 : 0;

            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = RollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            int smallStraightScore = sub.Length > 3 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 30 : 0;
            int largeStraightScore = sub.Length == 5 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 40 : 0;

            int chanceScore = (int)RollResult.Sum(x => x.Result);
            int yahtzeeScore = RollResult.GroupBy(x => x.Result).Count() == 1 ? 50 : 0;

            return new Dictionary<Category, int>
            {
                { Category.Aces, acesScore },
                { Category.Twos, twosScore },
                { Category.Threes, threesScore },
                { Category.Fours, foursScore },
                { Category.Fives, fivesScore },
                { Category.Sixes, sixesScore },
                { Category.ThreeOfKind, threeOfKindScore },
                { Category.FourOfKind, fourOfKindScore },
                { Category.FullHouse, fullHouseScore },
                { Category.SmallStraight, smallStraightScore },
                { Category.LargeStraight, largeStraightScore },
                { Category.Chance, chanceScore },
                { Category.Yahtzee, yahtzeeScore },
            };
        }

        public Dictionary<Category, int?>[] GameStatus()
        {
            return _gameStatus;
        }

        public void AddPoints(Category category)
        {
            if (_gameStatus[ActivePlayer][category]!=null)
                throw new ArgumentException($"Category {category} already taken! Choose other category.");

            _gameStatus[ActivePlayer][category] = CalculateScore(category);

            ActivePlayer++;
            RollsLeft = 3;
            if (ActivePlayer > Players.Length - 1)
            {
                ActivePlayer = 0;
            }
        }

        private int CalculateScore(Category category)
        {
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
            return RollResult.GroupBy(x => x.Result).Count() == 1 ? 50 : 0;
        }

        private int CalculateScoreForChance()
        {
            return RollResult.Sum(x => x.Result);
        }

        private int CalculateScoreForLargeStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = RollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            return sub.Length == 5 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                        .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 40 : 0;
        }

        private int CalculateScoreForSmallStraight()
        {
            var master = new int[] { 1, 2, 3, 4, 5, 6 };
            var sub = RollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            return sub.Length > 3 && master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                        .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 30 : 0;
        }

        private int CalculateScoreForFullHouse()
        {
            return RollResult.GroupBy(x => x.Result).Count() == 2 ? 25 : 0;
        }

        private int CalculateScoreForFourOfKind()
        {
            return RollResult.GroupBy(x => x.Result).Where(x => x.Count() == 4) != null ? RollResult.Sum(x => x.Result) : 0;
        }

        private int CalculateScoreForThreeOfKind()
        {
            return RollResult.GroupBy(x => x.Result).Where(x => x.Count() == 3) != null ? RollResult.Sum(x => x.Result) : 0;
        }

        private int CalculateScoreForGivenSideNumber(int side)
        {
            return RollResult.Where(x => x.Result == side).Sum(x => x.Result);
        }
    }
}