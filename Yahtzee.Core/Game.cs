using System;
using System.Collections.Generic;

namespace Yahtzee.Core
{
    public class Game
    {
        private readonly IRandomizer _randomizer;

        public List<string> Players { get; protected set; }
        public uint PlayerNameLength { get; set; } = 10;

        public Game(IRandomizer randomizer)
        {
            Players = new List<string>();
            _randomizer = randomizer;
        }

        public void AddPlayer(string playerName)
        {
            if (playerName.Length > PlayerNameLength)
            {
                throw new ArgumentException($"Max player name length is {PlayerNameLength}.");
            }
            if (Players.Count >= 4)
            {
                throw new ArgumentException($"Max number of players allowed: 4.");
            }
            Players.Add(playerName);
        }

        public int Roll()
        {
            return _randomizer.Roll(1, 6);
        }
    }
}