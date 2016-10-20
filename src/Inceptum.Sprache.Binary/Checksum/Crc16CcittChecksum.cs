using System;

namespace Inceptum.Sprache.Binary.Checksum
{
    enum Crc16CcittInitialValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

    class Crc16CcittChecksum : CrcChecksumBase<ushort>
    {
        private readonly ushort poly = 0x1021;
        private readonly ushort[] table = new ushort[256];
        private readonly ushort m_InitialValue = 0;

        public Crc16CcittChecksum(Crc16CcittInitialValue initialValue)
        {
            this.m_InitialValue = (ushort)initialValue;
            for (int i = 0; i < table.Length; ++i)
            {
                ushort temp = 0;
                var a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }

        protected override byte[] Convert(ushort crc)
        {
            return BitConverter.GetBytes(crc);
        }

        protected override ushort Calculate(byte[] bytes)
        {
            ushort crc = this.m_InitialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
    }
}