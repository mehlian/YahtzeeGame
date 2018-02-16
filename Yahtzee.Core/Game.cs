using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee.Core
{
    public class Game
    {
        private IRandomizer _randomizer;
        private Dictionary<Category, int?>[] _gameStatus;
        private YahtzeeScorer _yahtzeeScorer;
        private int[] _bonusScore;
        private int[] _partialScore;
        private int[] _totalScore;

        public string[] Players { get; protected set; }
        public int ActivePlayer { get; protected set; }
        public IDice[] RollResult { get; protected set; }
        public int RollsLeft { get; protected set; }
        public int[] PartialScore
        {
            get
            {
                int[] score = new int[Players.Length];
                for (int i = 0; i < Players.Length; i++)
                {
                    int? temp = 0;
                    temp += _gameStatus[ActivePlayer][Category.Aces];
                    temp += _gameStatus[ActivePlayer][Category.Twos];
                    temp += _gameStatus[ActivePlayer][Category.Threes];
                    temp += _gameStatus[ActivePlayer][Category.Fours];
                    temp += _gameStatus[ActivePlayer][Category.Fives];
                    temp += _gameStatus[ActivePlayer][Category.Sixes];
                    score[i] = temp == null ? 0 : (int)temp + BonusScore[i];
                }
                return score;
            }
            protected set { _partialScore = value; }
        }

        public int[] BonusScore
        {
            get
            {
                int[] score = new int[Players.Length];
                for (int i = 0; i < Players.Length; i++)
                {
                    int? temp = 0;
                    temp += _gameStatus[ActivePlayer][Category.Aces];
                    temp += _gameStatus[ActivePlayer][Category.Twos];
                    temp += _gameStatus[ActivePlayer][Category.Threes];
                    temp += _gameStatus[ActivePlayer][Category.Fours];
                    temp += _gameStatus[ActivePlayer][Category.Fives];
                    temp += _gameStatus[ActivePlayer][Category.Sixes];
                    score[i] = temp < 62 || temp == null ? 0 : 35;
                }
                return score;
            }
            protected set { _bonusScore = value; }
        }

        public int[] TotalScore
        {
            get
            {
                int[] score = new int[Players.Length];
                for (int i = 0; i < Players.Length; i++)
                {
                    int? temp = 0;
                    temp += _gameStatus[ActivePlayer][Category.Aces];
                    temp += _gameStatus[ActivePlayer][Category.Twos];
                    temp += _gameStatus[ActivePlayer][Category.Threes];
                    temp += _gameStatus[ActivePlayer][Category.Fours];
                    temp += _gameStatus[ActivePlayer][Category.Fives];
                    temp += _gameStatus[ActivePlayer][Category.Sixes];
                    temp += _gameStatus[ActivePlayer][Category.ThreeOfKind];
                    temp += _gameStatus[ActivePlayer][Category.FourOfKind];
                    temp += _gameStatus[ActivePlayer][Category.FullHouse];
                    temp += _gameStatus[ActivePlayer][Category.SmallStraight];
                    temp += _gameStatus[ActivePlayer][Category.LargeStraight];
                    temp += _gameStatus[ActivePlayer][Category.Yahtzee];
                    temp += _gameStatus[ActivePlayer][Category.Chance];

                    score[i] = temp == null ? 0 : (int)temp + BonusScore[i];
                }
                return score;
            }
            protected set { _totalScore = value; }
        }

        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
            _yahtzeeScorer = new YahtzeeScorer();
        }

        public void NewGame(params string[] playerName)
        {
            if (playerName.Length > 4)
                throw new ArgumentException("Max number of players is 4.");

            Players = playerName;
            ActivePlayer = 0;
            RollsLeft = 3;
            PartialScore = new int[playerName.Length];
            BonusScore = new int[playerName.Length];
            TotalScore = new int[playerName.Length];

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

        public Dictionary<Category, int> GetAvailableOptions()
        {
            var scores = new Dictionary<Category, int>();
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                if (_gameStatus[ActivePlayer][category] == null)
                    scores.Add(category, _yahtzeeScorer.CalculateScore(category, RollResult));
            }

            return scores;
        }

        public Dictionary<Category, int?>[] GameStatus()
        {
            return _gameStatus;
        }

        public void AddPoints(Category category)
        {
            if (_gameStatus[ActivePlayer][category] != null)
                throw new ArgumentException($"Category {category} already taken! Choose other category.");

            _gameStatus[ActivePlayer][category] = _yahtzeeScorer.CalculateScore(category, RollResult);

            ActivePlayer++;
            RollsLeft = 3;
            if (ActivePlayer > Players.Length - 1)
            {
                ActivePlayer = 0;
            }
        }
    }
}