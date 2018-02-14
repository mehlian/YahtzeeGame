using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee.Core
{
    public class Game
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 6;
        private IRandomizer _randomizer;
        private Dictionary<Category, CategoryStatus>[] _gameStatus;

        public string[] Players { get; protected set; }
        public int ActivePlayer { get; protected set; }
        public List<Dice> RollResult { get; protected set; }

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

            _gameStatus = new Dictionary<Category, CategoryStatus>[playerName.Length];
            for (int i = 0; i < playerName.Length; i++)
            {
                _gameStatus[i] = new Dictionary<Category, CategoryStatus>
                {
                    { Category.Aces, new CategoryStatus() },
                    { Category.Twos, new CategoryStatus() },
                    { Category.Threes, new CategoryStatus() },
                    { Category.Fours, new CategoryStatus() },
                    { Category.Fives, new CategoryStatus() },
                    { Category.Sixes, new CategoryStatus() },
                    { Category.ThreeOfKind, new CategoryStatus() },
                    { Category.FourOfKind, new CategoryStatus() },
                    { Category.FullHouse, new CategoryStatus() },
                    { Category.SmallStraight, new CategoryStatus()},
                    { Category.LargeStraight, new CategoryStatus() },
                    { Category.Chance, new CategoryStatus() },
                    { Category.Yahtzee, new CategoryStatus() },
                };
            }
        }

        public void RollDice(List<Dice> dice)
        {
            foreach (Dice die in dice)
            {
                if (die.IsUnlocked)
                {
                    die.Result = _randomizer.Roll(MIN_VALUE, MAX_VALUE);
                }
            }
            RollResult = dice;
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

            var master = new double[] { 1, 2, 3, 4, 5, 6 };
            var sub = RollResult.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();
            int smallStraightScore = master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
                .Take(sub.Length).DefaultIfEmpty().SequenceEqual(sub) ? 30 : 0;
            int largeStraightScore = master.SkipWhile((x, i) => !master.Skip(i).Take(sub.Length).SequenceEqual(sub))
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

        public Dictionary<Category, CategoryStatus>[] GameStatus()
        {
            return _gameStatus;
        }

        public void AddPoints(Category category)
        {
            _gameStatus[ActivePlayer][category].Points = 6;
        }
    }
}