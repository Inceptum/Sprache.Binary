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
        byte[] IChecksum.Calculate(byte[] bytes)
        {
            var crc = Calculate(bytes);
            var result = Convert(crc);
            return result;
        }

        int IChecksum.Length => Marshal.SizeOf<T>();

        protected abstract T Calculate(byte[] bytes);

        protected abstract byte[] Convert(T crc);
    }
}