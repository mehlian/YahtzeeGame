using System;
using System.Collections.Generic;

namespace Yahtzee.Core
{
    public class Game
    {
        private readonly IRandomizer _randomizer;
        private IList<Dice> _dices;

        public IList<string> Players { get; protected set; }
        public uint PlayerNameLength { get; set; }

        public Game(IRandomizer randomizer)
        {
            Players = new List<string>();
            PlayerNameLength = 10;
            _dices = MakeNewDiceSet();
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
            if (Players.Contains(playerName))
            {
                throw new ArgumentException($"Each player needs to have unique name. Try again with different name.");
            }
            Players.Add(playerName);
        }

        public void Roll()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!_dices[i].IsLocked)
                {
                    _dices[i].Result = _randomizer.Roll(1, 6);
                }
            }
        }

        public int RollResult(int dieNumber)
        {
            DieNumberValidation(dieNumber);

            return _dices[dieNumber - 1].Result;
        }

        public bool DieStatus(int dieNumber)
        {
            DieNumberValidation(dieNumber);

            return _dices[dieNumber - 1].IsLocked;
        }

        public void LockDie(int dieNumber)
        {
            DieNumberValidation(dieNumber);

            _dices[dieNumber - 1].IsLocked = true;
        }

        private List<Dice> MakeNewDiceSet()
        {
            return new List<Dice>
            {
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice(),
                new Dice()
            };
        }

        private void DieNumberValidation(int dieNumber)
        {
            if (dieNumber < 1 || dieNumber > 5)
                throw new ArgumentException("Incorect Die number.");
        }
    }
}