using System;

namespace Inceptum.Sprache.Binary
{
    internal class Input : IInput
    {
        private readonly byte[] m_Source;
        private readonly int m_Offset;
        private readonly int m_Count;

        public Input(byte[] source) : this(source, 0, source.Length)
        {
        }

        public Input(byte[] source, int offset, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (source.Length < offset)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset outside of buffer bounds");
            if (source.Length < offset + count)
                throw new ArgumentOutOfRangeException(nameof(count), "length outside of buffer bounds");

            m_Source = source;
            m_Offset = offset;
            m_Count = count;
        }

        public IInput Advance()
        {
            return new Input(m_Source, m_Offset + 1, m_Count - 1);
        }

        public byte[] Source => m_Source;

        public byte Current => m_Source[m_Offset];

        public bool AtEnd => m_Offset >= m_Source.Length || m_Count == 0;

        public int Position => m_Offset;

        protected bool Equals(Input other)
        {
            return Equals(m_Source, other.m_Source) && m_Offset == other.m_Offset;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Input) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((m_Source != null ? m_Source.GetHashCode() : 0)*397) ^ m_Offset;
            }
        }


    }
}