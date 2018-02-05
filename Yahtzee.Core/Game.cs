using System;
using System.Collections.Generic;

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
        public IDictionary<string, Table> Table { get;protected set; }

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

        public Dictionary<Category,int> GetAvailableOptions(string playerName)
        {
            
            return new Dictionary<Category, int>
            {
                { Category.Aces, 5 }
            }; 
        }
    }
}