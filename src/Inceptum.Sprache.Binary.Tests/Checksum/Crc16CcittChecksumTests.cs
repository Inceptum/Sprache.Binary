using System;
using System.Linq;
using Inceptum.Sprache.Binary.Checksum;
using NUnit.Framework;

namespace Inceptum.Sprache.Binary.Tests.Checksum
{
    [TestFixture]
    internal class Crc16CcittChecksumTests
    {
        private static void testChecksum(byte[] input, Crc16CcittInitialValue initialValue, ushort expected)
        {
            var checksum = new Crc16CcittChecksum(initialValue) as IChecksum;

            var crc = checksum.Calculate(input);
            var actual = BitConverter.ToUInt16(crc, 0);

            Assert.That(actual, Is.EqualTo(expected), string.Format("expected 0x{0:X} but was 0x{1:X}", expected, actual));
        }

        [Test]
        [TestCase(Crc16CcittInitialValue.NonZero1, (ushort) 0xFFFF)]
        [TestCase(Crc16CcittInitialValue.NonZero2, (ushort) 0x1D0F)]
        public void ChecksumForEmptyArratIsCorrect(Crc16CcittInitialValue initialValue, ushort expected)
        {
            byte[] input = new byte[] {};
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase(Crc16CcittInitialValue.NonZero1, (ushort) 0xB915)]
        [TestCase(Crc16CcittInitialValue.NonZero2, (ushort) 0x9479)]
        public void ChecksumForCharAArratIsCorrect(Crc16CcittInitialValue initialValue, ushort expected)
        {
            byte[] input = new byte[] {(byte) 'A'};
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase(Crc16CcittInitialValue.NonZero1, (ushort) 0x29B1)]
        [TestCase(Crc16CcittInitialValue.NonZero2, (ushort) 0xE5CC)]
        public void ChecksumForReferenceStringIsCorrect(Crc16CcittInitialValue initialValue, ushort expected)
        {
            var input = "123456789".ToCharArray().Select(c => (byte) c).ToArray();
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase(Crc16CcittInitialValue.NonZero1, (ushort)0xEA0B)]
        [TestCase(Crc16CcittInitialValue.NonZero2, (ushort)0xE938)]
        public void ChecksumFor255CharsAStringIsCorrect(Crc16CcittInitialValue initialValue, ushort expected)
        {
            var s = new string('A', 256);
            var input = s.ToCharArray().Select(c => (byte) c).ToArray();
            testChecksum(input, initialValue, expected);
        }
    }
}
