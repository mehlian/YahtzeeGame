using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Yahtzee.WinClient.UnitTests
{
    [TestFixture]
    public class DiceLockToVerticalAlignmentConverterTests
    {
        [Test]
        public void DiceLockToVerticalAlignmentConverter_CanBeCreated()
        {
            DiceLockToVerticalAlignmentConverter converter = new DiceLockToVerticalAlignmentConverter();
        }

        [Test]
        public void DiceLockToVerticalAlignmentConverter_ImplementsIValueConverter()
        {
            DiceLockToVerticalAlignmentConverter converter = new DiceLockToVerticalAlignmentConverter();

            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_DiceIsUnlocked_ReturnsTop()
        {
            DiceLockToVerticalAlignmentConverter converter = new DiceLockToVerticalAlignmentConverter();

            var result = (string)converter.Convert(true, null, null, null);
            var expected = "Top";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Convert_DiceIsUnlocked_ReturnsBottom()
        {
            DiceLockToVerticalAlignmentConverter converter = new DiceLockToVerticalAlignmentConverter();

            var result = (string)converter.Convert(false, null, null, null);
            var expected = "Bottom";

            Assert.AreEqual(expected, result);
        }
    }
}
