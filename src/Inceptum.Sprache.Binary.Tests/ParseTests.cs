﻿using System.Linq;
using NUnit.Framework;

namespace Inceptum.Sprache.Binary.Tests
{
    [TestFixture]
    internal class ParseTests
    {
        [Test]
        public void AcceptsByte()
        {
            var parser = Parse.Byte(0x01).Once();

            AssertHelpers.Success(
                parser, 
                new byte[] {0x01}, 
                new byte[] {0x01});
        }

        [Test]
        public void AcceptsOneByte()
        {
            var parser = Parse.Byte(0x01).Once();

            AssertHelpers.Success(
                parser,
                new byte[] { 0x01, 0x02 },
                new byte[] { 0x01 });
        }

        [Test]
        public void RejectsWrongByte()
        {
            var parser = Parse.Byte(0x01).Once();

            AssertHelpers.FailAt(
                parser,
                new byte[] { 0x02 },
                0);
        }

        [Test]
        public void RejectsEmptyInput()
        {
            AssertHelpers.FailAt(
                Parse.Byte(0x01).Once(),
                new byte[0],
                0);
        }

        [Test]
        public void AcceptsAnyOfBytes()
        {
            var parser = Parse.Bytes(0x01, 0x02, 0x03).Once();

            AssertHelpers.Success(parser, new byte[] { 0x01, }, new byte[] { 0x01 });
            AssertHelpers.Success(parser, new byte[] { 0x02, }, new byte[] { 0x02 });
            AssertHelpers.Success(parser, new byte[] { 0x03, }, new byte[] { 0x03 });
        }

        [Test]
        public void AcceptsSequenceOfBytes()
        {
            var parser = Parse.Sequence(0x01, 0x02, 0x03);

            AssertHelpers.Success(parser, new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x03 });
        }

        [Test]
        public void RejectsWrongSequenceOfBytes()
        {
            var parser = Parse.Sequence(0x01, 0x02, 0x03);

            AssertHelpers.FailAt(parser, new byte[] { 0x11, 0x12, 0x13 }, 0);
        }

        [Test]
        public void ConcatsResults()
        {
            var parser = Parse.Byte(0x01).Once().Then(a => Parse.Byte(0x02).Once().Select(b => a.Concat(b)));

            AssertHelpers.Success(parser, new byte[] { 0x01, 0x02 }, new byte[] { 0x01, 0x02 });
        }

        [Test]
        public void ParsesEmtpyInputWithMany()
        {
            AssertHelpers.Success(Parse.Byte(0x01).Many(), new byte[0], new byte[0]);
        }

        [Test]
        public void ParsesThreeBytesInputWithMany()
        {
            AssertHelpers.Success(Parse.Byte(0x01).Many(), new byte[3] {0x01, 0x01, 0x01}, new byte[3] { 0x01, 0x01, 0x01 });
        }

        [Test]
        public void ManyFailsWithWrongLastElement()
        {
            AssertHelpers.FailAt(Parse.Byte(0x01).Many().End(), new byte[3] { 0x01, 0x01, 0x02 }, 2);
        }

        [Test]
        public void ParserWithManyFailsWithWrongLastElement()
        {
            var parser = 
                from a in Parse.Byte(0x01).Once()
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);
            AssertHelpers.FailAt(parser.Many().End(), new byte[5] { 0x01, 0x02, 0x01, 0x02, 0x03 }, 4);
        }

        [Test]
        public void ParserWithXManyFailsWithWrongLastElement()
        {
            var parser =
                from a in Parse.Byte(0x01).Once()
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);
            AssertHelpers.FailAt(parser.Many().End(), new byte[5] { 0x01, 0x02, 0x01, 0x02, 0x03 }, 4);
        }

        [Test]
        public void AcceptsBytesWithLength()
        {
            var parser = 
                from a in Parse.Byte(0x01).Length(2).Once()
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);

            AssertHelpers.Success(parser, new byte[] { 0x01, 0xFF, 0x02 }, new byte[] { 0x01, 0x02 });
        }

        [Test]
        public void AcceptsBytesAndReturnsStartAndIndex()
        {
            var parser =
                from start in Parse.Index()
                from a in Parse.Byte(0x01).Length(2).Once()
                from b in Parse.Byte(0x02).Once()
                from end in Parse.Index()
                select new { start, end };

            var result = parser.Parse(new byte[] {0x01, 0xFF, 0x02});
            Assert.AreEqual(0, result.start);
            Assert.AreEqual(3, result.end);
        }

        [Test]
        public void AcceptsBytesAndReturnsSameIndex()
        {
            var parser =
                from start in Parse.Index()
                from end in Parse.Index()
                select new { start, end };

            var result = parser.Parse(new byte[] { 0x01, 0xFF, 0x02 });
            Assert.AreEqual(0, result.start);
            Assert.AreEqual(0, result.end);
        }

        [Test]
        public void WithSource()
        {
            var segment = 
                from a in Parse.Byte(0x01).Length(2).Once()
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);

            var parser =
                from x in segment.WithSource().XMany().End()
                select x;

            var result = parser.Parse(new byte[] {0x01, 0xFF, 0x02, 0x01, 0xAF, 0x02 }).ToArray();

            CollectionAssert.AreEqual(new byte[] {0x01, 0x02}, result[0].Value);
            CollectionAssert.AreEqual(new byte[] { 0x01, 0xFF, 0x02 }, result[0].Bytes);
            CollectionAssert.AreEqual(new byte[] {0x01, 0x02}, result[1].Value);
            CollectionAssert.AreEqual(new byte[] { 0x01, 0xAF, 0x02 }, result[1].Bytes);
        }

        [Test]
        public void AcceptsBytesAndReturnsIndexesFromInitialIntput()
        {
            var byteWithIndexes =
                from start in Parse.Index()
                from value in Parse.Byte(0x01)
                from end in Parse.Index()
                select new { start, value, end };

            var parser =
                from items in byteWithIndexes.Repeat(3).End()
                select items;

            var result = parser.Parse(new byte[] { 0x01, 0x01, 0x01 }).ToArray();
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, result[0].start);
            Assert.AreEqual(1, result[0].end);
            Assert.AreEqual(1, result[1].start);
            Assert.AreEqual(2, result[1].end);
            Assert.AreEqual(2, result[2].start);
            Assert.AreEqual(3, result[2].end);
        }

        [Test]
        public void FailsIfParserTriesToReadMoreThanLengthBytes()
        {
            var parser =
                from a in Parse.Sequence(0x01, 0x01, 0x01).Length(2)
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);

            AssertHelpers.FailAt(parser, new byte[] {0x01, 0x01, 0x01, 0xFF, 0x02}, 2);
        }

        [Test]
        public void FailsIfLengthIsGreaterThenInput()
        {
            var parser =
                from a in Parse.Byte(0x01).Once()
                from b in Parse.Byte(0x02).Once().Length(3)
                select a.Concat(b);

            AssertHelpers.FailAt(parser, new byte[] { 0x01, 0x02, 0x03 }, 1);
        }

        [Test]
        public void FailsIfLengthIsBeyondEndOfInput()
        {
            var parser =
                from a in Parse.Byte(0x01).Once()
                from b in Parse.Byte(0x02).Once()
                from c in Parse.Bytes(4)
                select a.Concat(b).Concat(c);

            AssertHelpers.FailAt(parser, new byte[] { 0x01, 0x02, 0x03 }, 2);
        }

        [Test]
        public void ParseUntilStopsOnTerminator()
        {
            var parser =
                from a in Parse.Byte().Until(Parse.Byte(0x00))
                select a;

            AssertHelpers.Success(parser, new byte[] { 0x31, 0x2e, 0x00 }, new byte[] { 0x31, 0x2e });
        }
    }
}
