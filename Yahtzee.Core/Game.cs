using System;
using System.Collections.Generic;

namespace Yahtzee.Core
{
    public class Game
    {
        public List<string> Players { get; protected set; }
        public uint PlayerNameLength { get; set; } = 10;

        public Game()
        {
            Players = new List<string>();
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
    }
}