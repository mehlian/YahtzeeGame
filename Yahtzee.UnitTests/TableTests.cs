using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class TableTests
    {
        private Table _table;

        [SetUp]
        public void Setup()
        {
            _table = new Table();
        }

        [Test]
        public void Table_CanBeCreated()
        {
            Table table = new Table();
        }

        //[Test]
        //public void Chance_GivenListOfDice_ReturnsUpdatedTable()
        //{
        //    List<Dice> dice = new List<Dice>
        //    {
        //        new Dice(){ Result = 1 },
        //        new Dice(){ Result = 1 },
        //        new Dice(){ Result = 1 },
        //        new Dice(){ Result = 1 },
        //        new Dice(){ Result = 1 },
        //    };

        //    Table result = _table.Update(dice);

        //    Assert.AreEqual(1, 1);
        //}
    }
}
