using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Yahtzee.WinClient.UnitTests
{
    [TestFixture]
    public class IntToImageConverterTests
    {
        [Test]
        public void IntToImageConverter_CanBeCreated()
        {
            IntToImageConverter converter = new IntToImageConverter();
        }

        [Test]
        public void IntToImageConverter_ImplementsIValueConverter()
        {
            IntToImageConverter converter = new IntToImageConverter();

            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void Convert_GivenNumber_ReturnsProperImage(int number)
        {
            IntToImageConverter converter = new IntToImageConverter();

            var result = (string)converter.Convert(number, null, null, null);
            var expected =$"pack://application:,,,/Yahtzee.WinClient;component/Images/dice{number}.png";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Convert_GivenIncorectNumber_ReturnsNull()
        {
            IntToImageConverter converter = new IntToImageConverter();

            var result = (string)converter.Convert(0, null, null, null);

            Assert.AreEqual(null, result);
        }
    }
}
