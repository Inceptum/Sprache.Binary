using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Inceptum.Sprache.Binary.Tests
{
    static class AssertHelpers
    {
        public static void Success(Parser<IEnumerable<byte>> parser, byte[] input, byte[] expected)
        {
            Success(parser, input, s => Assert.IsTrue(expected.SequenceEqual(s.Value)));
        }

        public static void Success<T>(Parser<T> parser, byte[] input, Action<IResult<T>> successAssertation)
        {
            parser.TryParse(input)
                .IfFailure(f =>
                {
                    Assert.Fail("Failed to parse input {0} with error {1}", input.Aggregate("0x", (a, b) => a + b.ToString("X2")), f.Message);

                    return f;
                })
                .IfSuccess(s =>
                {
                    successAssertation(s);
                    
                    return s;
                });
        }

        public static void FailAt<T>(Parser<T> parser, byte[] input, int position)
        {
            Fail(parser, input, result => Assert.AreEqual(position, result.Remainder.Position));
        }

        public static void Fail<T>(Parser<T> parser, byte[] input, Action<IResult<T>> failAssertation)
        {
            parser.TryParse(input)
                .IfFailure(f =>
                {
                    failAssertation(f);

                    return f;
                })
                .IfSuccess(s =>
                {
                    Assert.Fail("Expected to fail but succeded with result {0}", s.Value);

                    return s;
                });
        }
    }
}