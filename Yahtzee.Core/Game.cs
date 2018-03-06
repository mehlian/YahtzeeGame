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

        public string[] Players { get; protected set; }
        public int ActivePlayer { get; protected set; }
        public IDice[] RollResult { get; protected set; }
        public int RollsLeft { get; protected set; }
        public int?[] PartialScore { get; protected set; }
        public int?[] BonusScore { get; protected set; }
        public int?[] TotalScore { get; protected set; }
        public string GameWinner { get; protected set; }

        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
            _yahtzeeScorer = new YahtzeeScorer();
        }

        public void NewGame(params string[] playerName)
        {
            if (playerName.Length > 4)
                throw new ArgumentException("Max number of players is 4.");

            RestartGame(playerName);
        }

        private void RestartGame(string[] playerName)
        {
            var numberOfPlayers = playerName.Length;
            Players = playerName;
            ActivePlayer = 0;
            RollsLeft = 3;
            BonusScore = new int?[numberOfPlayers];
            PartialScore = new int?[numberOfPlayers];
            TotalScore = new int?[numberOfPlayers];

            _gameStatus = new Dictionary<Category, int?>[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
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
                throw new ArgumentException("Only set of five dice is supported.");

            foreach (var die in dice)
            {
                if (die.IsUnlocked)
                    die.Result = _randomizer.GetRandomNumber(1, die.SideNumber);
            }

            RollResult = dice;
            RollsLeft--;
        }

        public Dictionary<Category, int?> GetAvailableCategories()
        {
            var scores = new Dictionary<Category, int?>();
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                if (_gameStatus[ActivePlayer][category] == null)
                {
                    int[] rollResult = RollResult.Select(x => x.Result).ToArray();
                    scores.Add(category, _yahtzeeScorer.CalculateCategoryScore(category, rollResult));
                }
                else
                {
                    scores.Add(category, null);
                }
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

            int[] rollResult = RollResult.Select(x => x.Result).ToArray();
            _gameStatus[ActivePlayer][category] = _yahtzeeScorer.CalculateCategoryScore(category, rollResult);

            CalculateScore();

            ActivePlayer++;
            RollsLeft = 3;

            if (ActivePlayer > Players.Length - 1)
                ActivePlayer = 0;
        }

        private void CalculateScore()
        {
            PartialScore[ActivePlayer] = _yahtzeeScorer.CalculatePartialScore(_gameStatus[ActivePlayer]);
            BonusScore[ActivePlayer] = _yahtzeeScorer.CalculateBonusScore(_gameStatus[ActivePlayer]);
            TotalScore[ActivePlayer] = _yahtzeeScorer.CalculateTotalScore(_gameStatus[ActivePlayer]);
            GetWinner();
        }

        private void GetWinner()
        {
            if (TotalScore.All(x => x.HasValue))
            {
                for (int i = 0; i < Players.Length; i++)
                {
                    if (TotalScore[i].Value == TotalScore.Max())
                    {
                        GameWinner = Players[i];
                        break;
                    }
                }
            }
        }
    }
}