using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Inceptum.Sprache.Binary.Checksum;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Public static Sprache.Binary application program interface
    /// </summary>
    public static class Parse
    {
        #region Parser Helpers
        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {

            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (convert == null) throw new ArgumentNullException(nameof(convert));

            return parser.Then(t => Return(convert(t)));
        }

        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }
        #endregion

        public static Parser<T> Length<T>(this Parser<T> parser, int length)
        {
            return i =>
            {
                var start = i.Position;
                var r = parser(new Input(i.Source, i.Position, length));
                var end = r.Remainder.Position;
                if (r.WasSuccessful)
                {
                    return Result.Success(r.Value, i.Skip(length));
                }
                return r;
            };
        }

        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        public static Parser<byte> Bytes(params byte[] bytes)
        {
            return Byte(bytes.Contains, string.Join("|", bytes.Select(b => b.ToString("X"))));
        }

        public static Parser<IEnumerable<byte>> Sequence(params byte[] bytes)
        {
            return bytes.Select(Byte).Aggregate(Return(Enumerable.Empty<byte>()), (a, p) => a.Concat(p.Once()));
        }

        public static Parser<IEnumerable<byte>> Bytes(int length)
        {
            return Byte().Repeat(length);
        }

        public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(r => (IEnumerable<T>)new[] { r });
        }

        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)
        {
            return first.Then(f => second.Select(f.Concat));
        }

        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            return Repeat(parser, count, count);
        }

        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int minimumCount, int maximumCount)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var remainder = i;
                var result = new List<T>();

                for (var n = 0; n < maximumCount; ++n)
                {
                    var r = parser(remainder);

                    if (!r.WasSuccessful && n < minimumCount)
                    {
                        var what = r.Remainder.AtEnd
                            ? "end of input"
                            : r.Remainder.Current.ToString();

                        var msg = $"Unexpected '{what}'";
                        var exp = $"'{string.Join(", ", r.Expectations)}' between {minimumCount} and {maximumCount} times, but found {n}";

                        return Result.Failure<IEnumerable<T>>(i, msg, new[] { exp });
                    }

                    if (remainder != r.Remainder)
                    {
                        result.Add(r.Value);
                    }

                    remainder = r.Remainder;
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        public static Parser<byte> Byte(byte b)
        {
            return Byte(bt => bt == b, $"{b:X}");
        }

        public static Parser<byte> Byte(byte min, byte max)
        {
            return Byte(bt => min <= bt && bt <= max , $"between {min:X} and {max:X}");
        }

        public static Parser<byte> Byte()
        {
            return Byte(bt => true, "any byte");
        }

        public static Parser<byte> Byte(Predicate<byte> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (description == null) throw new ArgumentNullException(nameof(description));

            return i =>
            {
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return Result.Success(i.Current, i.Advance());

                    return Result.Failure<byte>(i, $"unexpected '{i.Current:X}'", new[] { description });
                }

                return Result.Failure<byte>(i, "Unexpected end of input reached", new[] { description });
            };
        }

        public static Parser<T> WithChecksum<T>(this Parser<T> parser, IChecksum checksum)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var start = i.Position;
                var a = parser(i);
                var end = a.Remainder.Position;

                return a.IfSuccess(
                    b =>
                    {
                        return Parse.Bytes(checksum.Length)(a.Remainder)
                            .IfSuccess(
                                c =>
                                {
                                    var expected = checksum.Calculate(a.Remainder.Source.Skip(start).Take(end - start).ToArray());
                                    var actual = c.Value.ToArray();
                                    return expected.SequenceEqual(actual)
                                        ? Result.Success(b.Value, c.Remainder)
                                        : Result.Failure<T>(b.Remainder, "invalid checksum", new[] {$"expected checksum {BitConverter.ToString(expected)} but was {BitConverter.ToString(actual)}"});
                                }
                            );
                    });
            };
        }
        public static Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)
        {

            return parser.Many().Then(m => parser.Once().XOr(Return(m)));
        }

        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var r = parser(i);

                while (r.WasSuccessful)
                {
                    if (remainder.Equals(r.Remainder))
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        public static Parser<T> End<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i => parser(i).IfSuccess(s => s.Remainder.AtEnd ? s : Result.Failure<T>(s.Remainder, $"unexpected '{s.Remainder.Current:X}'", new[] { "end of input" }));
        }

        public static Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    // The 'X' part
                    if (!fr.Remainder.Equals(i))
                        return fr;

                    return second(i).IfFailure(sf => DetermineBestError(fr, sf));
                }

                // This handles a zero-length successful application of first.
                if (fr.Remainder.Equals(i))
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        static IResult<T> DetermineBestError<T>(IResult<T> firstFailure, IResult<T> secondFailure)
        {
            if (secondFailure.Remainder.Position > firstFailure.Remainder.Position)
                return secondFailure;

            if (secondFailure.Remainder.Position == firstFailure.Remainder.Position)
                return Result.Failure<T>(
                    firstFailure.Remainder,
                    firstFailure.Message,
                    firstFailure.Expectations.Union(secondFailure.Expectations));

            return firstFailure;
        }

        public static Parser<Int64> Int64()
        {
            return parse(BitConverter.ToInt64);
        }

        public static Parser<Int32> Int32()
        {
            return parse(BitConverter.ToInt32);
        }

        public static Parser<Int16> Int16()
        {
            return parse(BitConverter.ToInt16);
        }
        public static Parser<UInt64> UInt64()
        {
            return parse(BitConverter.ToUInt64);
        }

        public static Parser<UInt32> UInt32()
        {
            return parse(BitConverter.ToUInt32);
        }

        public static Parser<UInt16> UInt16()
        {
            return parse(BitConverter.ToUInt16);
        }
        
        public static Parser<SByte> SByte()
        {
            return parse((bytes, i) => (sbyte)bytes[0]);
        }

        private static Parser<T> parse<T>(Func<byte[], int, T> converter)
            where T : struct
        {
            return from bts in Parse.Bytes(Marshal.SizeOf<T>())
                   let bfr = bts.ToArray()
                   select converter(bfr, 0);
        }
    }
}