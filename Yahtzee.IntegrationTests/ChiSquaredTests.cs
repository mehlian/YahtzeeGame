using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.IntegrationTests
{
    [TestFixture]
    public class ChiSquaredTests
    {
        [Test]
        public void ChiSquared_CanBeCreated()
        {
            ChiSquared chiSquared = new ChiSquared();
        }

    }
}
