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
    public class PlayerTests
    {
        [Test]
        public void Player_CanBeInitialized()
        {
            Player player = new Player();
        }

        [Test]
        public void Name_ForGivenPlayersName_ReturnsName()
        {
            Player player = new Player();
            string randomName = "a";

            player.Name = randomName;

            Assert.AreEqual(randomName, player.Name);
        }

        [Test]
        public void Score_ForGivenScore_ReturnsScore()
        {
            Player player = new Player();
            int score = 1;

            player.Score = score;

            Assert.AreEqual(score, player.Score);
        }
    }
}
