namespace Inceptum.Sprache.Binary.Checksum
{
    /// <summary>
    /// Public contract to inject checksum calculation into <see cref="Parser{T}"/> class via <see cref="Parse"/> WithChecksum method
    /// </summary>
    public interface IChecksum
    {
        /// <summary>
        /// Calculates checksum for provided array of bytes
        /// </summary>
        /// <param name="bytes">Array of bytes to calculate checksum on</param>
        /// <returns>Checksum bytes</returns>
        byte[] Calculate(byte[] bytes);

        /// <summary>
        /// Length of checksum
        /// </summary>
        int Length { get; }
    }
}