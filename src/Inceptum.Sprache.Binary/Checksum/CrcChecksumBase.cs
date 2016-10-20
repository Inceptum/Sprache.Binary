using System.Runtime.InteropServices;

namespace Inceptum.Sprache.Binary.Checksum
{
    /// <summary>
    /// Abstract base class to calculate checksum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CrcChecksumBase<T> : IChecksum
        where T : struct
    {
        private static readonly int m_SizeOf = Marshal.SizeOf(typeof(T));

        byte[] IChecksum.Calculate(byte[] bytes)
        {
            var crc = Calculate(bytes);
            var result = Convert(crc);
            return result;
        }

        int IChecksum.Length
        {
            get { return m_SizeOf; }
        }

        protected abstract T Calculate(byte[] bytes);

        protected abstract byte[] Convert(T crc);
    }
}