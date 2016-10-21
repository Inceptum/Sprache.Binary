using System.Linq;
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
        public void FailsIfParserTriesToReadMoreThanLengthBytes()
        {
            var parser =
                from a in Parse.Sequence(0x01, 0x01, 0x01).Length(2)
                from b in Parse.Byte(0x02).Once()
                select a.Concat(b);

            AssertHelpers.FailAt(parser, new byte[] {0x01, 0x01, 0x01, 0xFF, 0x02}, 2);
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
