using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee.Core
{
    public class Game
    {
        private IRandomizer _randomizer;
        private Dictionary<Category, int?>[] _gameStatus;
        private bool[] _hasYahtzee;

        public Game(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public string[] Players { get; protected set; }
        public int ActivePlayer { get; protected set; }
        public IDice[] RollResult { get; protected set; }
        public int RollsLeft { get; protected set; }
        public int?[] PartialScore { get; protected set; }
        public int?[] BonusScore { get; protected set; }
        public int?[] TotalScore { get; protected set; }
        public string GameWinner { get; protected set; }

        public void NewGame(params string[] playerName)
        {
            if (playerName.Length > 4)
                throw new ArgumentException("Max number of supported players is 4.");

            RestartGame(playerName);
        }
        public void RollDice(IDice[] dice)
        {
            if (RollsLeft < 1)
                throw new InvalidOperationException($"Player {Players[ActivePlayer]} has exceeded the maximum number of dice rolls in this turn.");

            if (dice.Length != 5)
                throw new RankException("Only rank of 5 dices is supported.");

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
                    var scorer = ScorerFactory.GetScorer(_hasYahtzee[ActivePlayer]);

                    scores.Add(category, scorer.CalculateCategoryScore(category, rollResult));
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
            var scorer = ScorerFactory.GetScorer(_hasYahtzee[ActivePlayer]);
            _gameStatus[ActivePlayer][category] = scorer.CalculateCategoryScore(category, rollResult);

            CalculateScore();

            if (!_hasYahtzee[ActivePlayer] && _gameStatus[ActivePlayer][Category.Yahtzee] == 50)
            {
                _hasYahtzee[ActivePlayer] = true;
            }
            else if(_hasYahtzee[ActivePlayer] && new RegularScorer().CalculateCategoryScore(Category.Yahtzee, rollResult) == 50)
            {
                _gameStatus[ActivePlayer][Category.Yahtzee] += 100;
            }

            ActivePlayer++;
            RollsLeft = 3;

            if (ActivePlayer > Players.Length - 1)
                ActivePlayer = 0;
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
            _hasYahtzee = new bool[numberOfPlayers];

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
        private void CalculateScore()
        {
            var scorer = ScorerFactory.GetScorer(_hasYahtzee[ActivePlayer]);
            PartialScore[ActivePlayer] = scorer.CalculatePartialScore(_gameStatus[ActivePlayer]);
            BonusScore[ActivePlayer] = scorer.CalculateBonusScore(_gameStatus[ActivePlayer]);
            TotalScore[ActivePlayer] = scorer.CalculateTotalScore(_gameStatus[ActivePlayer]);
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