namespace Inceptum.Sprache.Binary
{
    /// <summary>
    /// Input interface for binary parser
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Next input
        /// </summary>
        /// <returns>Next input</returns>
        IInput Advance();

        /// <summary>
        /// Source
        /// </summary>
        byte[] Source { get; }

        /// <summary>
        /// Byte value at current position
        /// </summary>
        byte Current { get; }

        /// <summary>
        /// Indicates if input is ended
        /// </summary>
        bool AtEnd { get; }

        /// <summary>
        /// Current position
        /// </summary>
        int Position { get; }
    }

    /// <summary>
    /// Input extensions
    /// </summary>
    internal static class InputExtensions
    {
        /// <summary>
        /// Skips <param name="count"></param> of bytes
        /// </summary>
        /// <param name="input">Input to skip bytes at</param>
        /// <param name="count">Number of bytes to skip</param>
        /// <returns>Input with current position right after skipped bytes</returns>
        public static IInput Skip(this IInput input, int count)
        {
            while (count-- > 0)
                input = input.Advance();

            return input;
        }
    }
}