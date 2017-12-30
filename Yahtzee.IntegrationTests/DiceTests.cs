using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
   public class DiceTests
    {
        [Test]
        public void Roll_For6000TriesDiceShouldBeFair_ReturnsTrue()
        {
            IRandomizer randomizer = new Randomizer();
            Dice dice = new Dice(randomizer);
            ChiSquared chiSquared = new ChiSquared();
            double[] observed = new double[6];
            double[] expected = {1000, 1000, 1000, 1000, 1000, 1000 };
            double alfa = 0.01;

            for (int i = 0; i < 6000; i++)
            {
                dice.Roll();
                observed[dice.Result - 1]++;
            }

            bool DieIsFair = chiSquared.CheckPValue(observed, expected, alfa);

            Assert.IsTrue(DieIsFair);
        }
    }
}
