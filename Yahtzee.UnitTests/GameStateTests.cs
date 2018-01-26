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
    public class GameStateTests
    {
        [Test]
        public void GameState_CanBeInitialized()
        {
            GameState gameState = new GameState();
        }

    }
}
