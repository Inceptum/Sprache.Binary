namespace Inceptum.Sprache.Binary.Checksum
{
    /// <summary>
    /// CRC 16 CCITT 
    /// </summary>
    public static class Crc16Ccitt
    {
        /// <summary>
        /// CRC 16 CCITT with initial value set to 0x0000
        /// </summary>
        public static readonly IChecksum Zeros = new Crc16CcittChecksum(Crc16CcittInitialValue.Zeros);

        /// <summary>
        /// CRC 16 CCITT with initial value set to 0xffff
        /// </summary>
        public static readonly IChecksum NonZero1 = new Crc16CcittChecksum(Crc16CcittInitialValue.NonZero1);

        /// <summary>
        /// CRC 16 CCITT with initial value set to  0x1D0F
        /// </summary>
        public static readonly IChecksum NonZero2 = new Crc16CcittChecksum(Crc16CcittInitialValue.NonZero2);
    }
}