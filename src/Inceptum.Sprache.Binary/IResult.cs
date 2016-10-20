using System.Collections.Generic;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Parse result data object
    /// </summary>
    /// <typeparam name="T">Type data is parsed to</typeparam>
    public interface IResult<out T>
    {
        /// <summary>
        /// Gets the value indicating the parse was successfull
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Gets the resulting value of parser
        /// </summary>
        T Value { get; }
        
        /// <summary>
        /// Gets the error message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the remainder of the input
        /// </summary>
        IInput Remainder { get; }

        /// <summary>
        /// Gets expected input description
        /// </summary>
        IEnumerable<string> Expectations { get; }
    }
}