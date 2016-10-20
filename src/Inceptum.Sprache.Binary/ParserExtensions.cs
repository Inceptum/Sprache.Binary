using System;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Public static <see cref="Parser{T}"/> application program interface
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Tries to parse input to instnce of type T
        /// </summary>
        /// <typeparam name="T">Parse result type</typeparam>
        /// <param name="parser"><see cref="Parser{T}"/> instance</param>
        /// <param name="input">Input to parse</param>
        /// <returns>Parse result <see cref="IResult{T}"/></returns>
        public static IResult<T> TryParse<T>(this Parser<T> parser, byte[] input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            return parser(new Input(input));
        }

        /// <summary>
        /// Parse input to instnce of type T
        /// </summary>
        /// <typeparam name="T">Parse result type</typeparam>
        /// <param name="parser"><see cref="Parser{T}"/> instance</param>
        /// <param name="input">Input to parse</param>
        /// <returns>Parsed result of type T</returns>
        /// <exception cref="ParseException">Throws exeption if parse was not successfull</exception>
        public static T Parse<T>(this Parser<T> parser, byte[] input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            var result = parser.TryParse(input);

            if (result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }
    }
}