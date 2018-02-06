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
        // private IDictionary<Category, int> _column;
        private IDictionary<string, Table> _table;

        public string[] Players { get; protected set; }
        public List<Dice> RollResult { get; protected set; }
        public IDictionary<string, Table> Table { get; protected set; }

        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
            _table = new Dictionary<string, Table>();
        }

        public void NewGame(string[] playerName)
        {
            if (playerName.Length > 4)
                throw new ArgumentException("Max number of players is 4.");

            Players = playerName;
            for (int i = 0; i < playerName.Length; i++)
            {
                _table.Add(playerName[i], new Table());
            }
            Table = _table;
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

            var threeOfKind = RollResult.GroupBy(x => x.Result);
            int threeOfKindScore = 0;
            foreach (var group in threeOfKind)
            {
                if (group.Count() == 3)
                {
                    threeOfKindScore = (int)RollResult.Sum(x => x.Result);
                    break;
                }
            }

            var fourOfKind = RollResult.GroupBy(x => x.Result);
            int fourOfKindScore = 0;
            foreach (var group in fourOfKind)
            {
                if (group.Count() == 4)
                {
                    fourOfKindScore = (int)RollResult.Sum(x => x.Result);
                    break;
                }
            }

            var fullHouse = RollResult.GroupBy(x => x.Result).Count();
            int fullHouseScore = 0;
            if (fullHouse == 2)
            {
                fullHouseScore = 25;
            }

            var smallStraight = RollResult.Select(x => x.Result).Distinct().OrderByDescending(x => x).ToArray();
            int smallStraightScore = 0;
            if (smallStraight.SequenceEqual(new double[] { 4, 3, 2, 1 }) || 
                smallStraight.SequenceEqual(new double[] { 5, 4, 3, 2 }) || 
                smallStraight.SequenceEqual(new double[] { 6, 5, 4, 3 }))
            {
                smallStraightScore = 30;
            }

            var largeStraight = RollResult.Select(x => x.Result).Distinct().OrderByDescending(x => x).ToArray();
            int largeStraightScore = 0;
            if (largeStraight.SequenceEqual(new double[] { 5, 4, 3, 2, 1 }) ||
                largeStraight.SequenceEqual(new double[] { 6, 5, 4, 3,2 }))
            {
                largeStraightScore = 40;
            }

            int chanceScore = (int)RollResult.Sum(x => x.Result);

            int yahtzee = RollResult.GroupBy(x => x.Result).Count();
            int yahtzeeScore = 0;
            if (yahtzee==1)
            {
                yahtzeeScore = 50;
            }

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
    }
}