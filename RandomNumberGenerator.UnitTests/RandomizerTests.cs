using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Core;

namespace RandomNumberGenerator.UnitTests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void Randomizer_CanBeCreated()
        {
            Randomizer randomizer = new Randomizer();
        }

        [Test]
        public void Randomizer_ImplementsIRandomizer()
        {
            Randomizer randomizer = new Randomizer();

            Assert.IsInstanceOf<IRandomizer>(randomizer);
        }
    }
}
