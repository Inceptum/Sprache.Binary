using System;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Parse exception
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Creates new instance of <see cref="ParseException"/>
        /// </summary>
        public ParseException(string message) : base(message)
        {
        }
    }
}