using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void Game_ForGivenPlayersNames_CanBeInitialized()
        {
            Game game = new Game();
        }

        //[Test]
        //public void AddPlayer_ForGivenPlayerName_AddPlayerToListOfPlayers()
        //{
        //    Game game = new Game();
        //    string playerToAdd = "A";
        //    List<string>
        //    game.AddPlayer(playerToAdd);

        //    Assert.AreEqual(playerToAdd, game.Players);
        //}
    }
}
