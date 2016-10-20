using System;
using System.Collections.Generic;
using System.Linq;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Public static <see cref="IResult{T}"/> application program interface
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates successfull parser result of T
        /// </summary>
        /// <typeparam name="T">Type of parse result</typeparam>
        /// <param name="value">Value of parse result</param>
        /// <param name="remainder">Input advanced right after parse of result is finished</param>
        /// <returns>Instance of Success <see cref="IResult{T}"/> advanced to right after parse of result is finished</returns>
        public static IResult<T> Success<T>(T value, IInput remainder)
        {
            return new Result<T>(value, remainder);
        }

        /// <summary>
        /// Creates failure parser result of T
        /// </summary>
        /// <typeparam name="T">Type of parse result</typeparam>
        /// <param name="remainder">Input advanced right after parse of result is finished</param>
        /// <param name="message">Message</param>
        /// <param name="expectations">Expectations</param>
        /// <returns>Instance of Failure <see cref="IResult{T}"/> advanced to right after parse of result is finished</returns>
        public static IResult<T> Failure<T>(IInput remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    internal class Result<T> : IResult<T>
    {
        private readonly T m_Value;

        public Result(T value, IInput remainder)
        {
            m_Value = value;
            Remainder = remainder;
            WasSuccessful = true;
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            Remainder = remainder;
            Message = message;
            Expectations = expectations;
            WasSuccessful = false;
        }

        public bool WasSuccessful { get; private set; }

        public T Value
        {
            get
            {
                if(!WasSuccessful)
                    throw new InvalidOperationException("No value was parsed");

                return m_Value;
            }
        }

        public string Message { get; private set; }

        public IInput Remainder { get; private set; }

        public IEnumerable<string> Expectations { get; private set; }

        public override string ToString()
        {
            if (WasSuccessful)
                return string.Format("Successful parsing of {0}.", Value);

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);
            
            return string.Format("Parsing failure: {0};{1} ({2});", Message, expMsg, Remainder);
        }
    }
}