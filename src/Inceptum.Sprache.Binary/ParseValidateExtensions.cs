using System;
using System.Collections.Generic;
using System.Linq;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Public static <see cref="Parser{T}"/> application program interface for sprcific types
    /// </summary>
    public static class ParseValidateExtensions
    {
        /// <summary>
        /// Parses next two bytes to UInt16 
        /// </summary>
        /// <param name="parser">Instance of bytes parser</param>
        /// <returns>Two bytes converted to UInt16</returns>
        public static Parser<ushort> ToUInt16(this Parser<IEnumerable<byte>> parser)
        {
            return i =>
            {
                return parser(i).IfSuccess(r =>
                {
                    var bytes = r.Value.ToArray();
                    if (bytes.Length != 2)
                        return Result.Failure<UInt16>(i, $"expected 2 bytes but was ${bytes.Length}", new string[] { "0..999" });

                    return Result.Success(BitConverter.ToUInt16(bytes, 0), r.Remainder);
                });

            };
        }

        /// <summary>
        /// Validates that parsed UInt16 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of UInt16 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<ushort> Range(this Parser<ushort> parser, ushort min, ushort max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        /// <summary>
        /// Validates that parsed UInt32 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of UInt32 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<uint> Range(this Parser<uint> parser, uint min, uint max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        /// <summary>
        /// Validates that parsed UInt64 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of UInt64 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<ulong> Range(this Parser<ulong> parser, ulong min, ulong max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        /// <summary>
        /// Validates that parsed Int16 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of Int16 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<short> Range(this Parser<short> parser, short min, short max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        /// <summary>
        /// Validates that parsed Int32 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of Int32 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<int> Range(this Parser<int> parser, int min, int max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        /// <summary>
        /// Validates that parsed Int64 value is between min and max range
        /// </summary>
        /// <param name="parser">Instance of Int64 parser</param>
        /// <param name="min">Parsed value must be greater or equal to min</param>
        /// <param name="max">Parsed value must be less or equal to max</param>
        /// <returns>Not changed instance of parser for further chaining</returns>
        public static Parser<long> Range(this Parser<long> parser, long min, long max)
        {
            return Validate(parser, val => min <= val && val <= max, $"between {min} and {max}");
        }

        public static Parser<T> Validate<T>(this Parser<T> parser, Func<T, bool> predicate, string description)
        {
            return i =>
            {
                return parser(i).IfSuccess(r => predicate(r.Value) ? r : Result.Failure<T>(i, "not valid", new[] { description }));
            };
        }
    }
}