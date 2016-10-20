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
        private readonly IInput m_Remainder;
        private readonly string m_Message;
        private readonly IEnumerable<string> m_Expectations;
        private readonly bool m_WasSuccessful ;

        public Result(T value, IInput remainder)
        {
            m_Value = value;
            m_Remainder = remainder;
            m_WasSuccessful = true;
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            m_Remainder = remainder;
            m_Message = message;
            m_Expectations = expectations;
            m_WasSuccessful = false;
        }

        public bool WasSuccessful => m_WasSuccessful;

        public T Value
        {
            get
            {
                if(!WasSuccessful)
                    throw new InvalidOperationException("No value was parsed");

                return m_Value;
            }
        }
        public string Message => m_Message;
        public IInput Remainder => m_Remainder;
        public IEnumerable<string> Expectations => m_Expectations;

        public override string ToString()
        {
            if (WasSuccessful)
                return $"Successful parsing of {Value}.";

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);
            
            return $"Parsing failure: {Message};{expMsg} ({Remainder});";
        }
    }
}