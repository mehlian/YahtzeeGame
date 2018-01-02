using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Yahtzee.Core;

namespace Yahtzee.UnitTests
{
    [TestFixture]
    public class TableTests
    {
        public Table MakeTable()
        {
            return new Table();
        }

        [Test]
        public void Table_CanBeInitialized()
        {
            Table table = MakeTable();
        }

        [Test]
        public void Aces_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Aces = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Aces);
        }

        [Test]
        public void Twos_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table =  MakeTable();
            int pointsToStore = 1;

            table.Twos = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Twos);
        }

        [Test]
        public void Threes_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Threes = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Threes);
        }

        [Test]
        public void Fours_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Fours = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Fours);
        }

        [Test]
        public void Fives_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Fives = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Fives);
        }

        [Test]
        public void Sixes_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Sixes = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Sixes);
        }

        [Test]
        public void ThreeOfAKind_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.ThreeOfAKind = pointsToStore;

            Assert.AreEqual(pointsToStore, table.ThreeOfAKind);
        }

        [Test]
        public void FourOfAKind_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.FourOfAKind = pointsToStore;

            Assert.AreEqual(pointsToStore, table.FourOfAKind);
        }

        [Test]
        public void FullHouse_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.FullHouse = pointsToStore;

            Assert.AreEqual(pointsToStore, table.FullHouse);
        }

        [Test]
        public void SmallStraight_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.SmallStraight = pointsToStore;

            Assert.AreEqual(pointsToStore, table.SmallStraight);
        }

        [Test]
        public void LargeStraigth_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.LargeStraight = pointsToStore;

            Assert.AreEqual(pointsToStore, table.LargeStraight);
        }

        [Test]
        public void Yahtzee_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Yahtzee = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Yahtzee);
        }

        [Test]
        public void Chance_ForGivenPoints_ReturnsStoredPoints()
        {
            Table table = MakeTable();
            int pointsToStore = 1;

            table.Chance = pointsToStore;

            Assert.AreEqual(pointsToStore, table.Chance);
        }
    }
}
