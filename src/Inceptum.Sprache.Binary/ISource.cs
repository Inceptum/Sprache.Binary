using System;

namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Parse value with source bytes
    /// </summary>
    public class Source<T> : Tuple<T, byte[]>
    {
        public Source(T value, byte[] bytes)
            : base(value, bytes)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (bytes == null) throw new ArgumentNullException("bytes");
        }

        /// <summary>
        /// Parsed value
        /// </summary>
        public T Value
        {
            get { return base.Item1; }
        }

        /// <summary>
        /// Source bytes
        /// </summary>
        public byte[] Bytes
        {
            get { return base.Item2; }
        }
    }
}