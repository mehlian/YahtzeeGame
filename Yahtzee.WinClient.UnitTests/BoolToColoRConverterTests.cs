using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Yahtzee.WinClient.UnitTests
{
    [TestFixture]
    public class BoolToColoRConverterTests
    {
        [Test]
        public void BoolToColorConverter_CanBeCreated()
        {
            BoolToColorConverter converter = new BoolToColorConverter();
        }

        [Test]
        public void BoolToColorConverter_ImplementsIValueConverter()
        {
            BoolToColorConverter converter = new BoolToColorConverter();

            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_FalseAsValue_ReturnsTransparentBrush()
        {
            BoolToColorConverter converter = new BoolToColorConverter();

            var expected = new SolidColorBrush(Color.FromArgb(0, 255, 255, 210)).Color;
            var result = ((SolidColorBrush)converter.Convert(false, null, null, null)).Color;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Convert_TrueAsValue_ReturnsLightGoldenrodYellowBrush()
        {
            BoolToColorConverter converter = new BoolToColorConverter();

            var expected = new SolidColorBrush(Color.FromArgb(255, 255, 255, 210)).Color;
            var result = ((SolidColorBrush)converter.Convert(true, null, null, null)).Color;

            Assert.AreEqual(expected, result);
        }
    }
}
