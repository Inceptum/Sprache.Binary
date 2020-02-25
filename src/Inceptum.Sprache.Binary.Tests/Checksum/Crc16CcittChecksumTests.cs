using System;
using System.Collections.Generic;
using System.Linq;
using Inceptum.Sprache.Binary.Checksum;
using NUnit.Framework;

namespace Inceptum.Sprache.Binary.Tests.Checksum
{
    [TestFixture]
    internal class Crc16CcittChecksumTests
    {
        private static Dictionary<string, IChecksum> m_Map = new Dictionary<string, IChecksum>
        {
            {"NonZero1", Crc16Ccitt.NonZero1},
            {"NonZero2", Crc16Ccitt.NonZero2},
            {"Zeros", Crc16Ccitt.Zeros},
        };
        private static void testChecksum(byte[] input, string initialValue, ushort expected)
        {
            var checksum = m_Map[initialValue];

            var crc = checksum.Calculate(input);
            var actual = BitConverter.ToUInt16(crc, 0);

            Assert.That(actual, Is.EqualTo(expected), string.Format("expected 0x{0:X} but was 0x{1:X}", expected, actual));
        }

        [Test]
        [TestCase("NonZero1", (ushort) 0xFFFF)]
        [TestCase("NonZero2", (ushort) 0x1D0F)]
        public void ChecksumForEmptyArratIsCorrect(string initialValue, ushort expected)
        {
            byte[] input = new byte[] {};
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase("NonZero1", (ushort) 0xB915)]
        [TestCase("NonZero2", (ushort) 0x9479)]
        public void ChecksumForCharAArratIsCorrect(string initialValue, ushort expected)
        {
            byte[] input = new byte[] {(byte) 'A'};
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase("NonZero1", (ushort) 0x29B1)]
        [TestCase("NonZero2", (ushort) 0xE5CC)]
        public void ChecksumForReferenceStringIsCorrect(string initialValue, ushort expected)
        {
            var input = "123456789".ToCharArray().Select(c => (byte) c).ToArray();
            testChecksum(input, initialValue, expected);
        }

        [Test]
        [TestCase("NonZero1", (ushort)0xEA0B)]
        [TestCase("NonZero2", (ushort)0xE938)]
        public void ChecksumFor255CharsAStringIsCorrect(string initialValue, ushort expected)
        {
            var s = new string('A', 256);
            var input = s.ToCharArray().Select(c => (byte) c).ToArray();
            testChecksum(input, initialValue, expected);
        }
    }
}
