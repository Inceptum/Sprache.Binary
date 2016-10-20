using System;

namespace Inceptum.Sprache.Binary
{
    internal class Input : IInput
    {
        private readonly int m_Offset;
        private readonly int m_Count;

        public Input(byte[] source) : this(source, 0, source.Length)
        {
        }

        public Input(byte[] source, int offset, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (source.Length < offset)
                throw new ArgumentOutOfRangeException("offset", "offset outside of buffer bounds");
            if (source.Length < offset + count)
                throw new ArgumentOutOfRangeException("count", "length outside of buffer bounds");

            Source = source;
            m_Offset = offset;
            m_Count = count;
        }

        public IInput Advance()
        {
            return new Input(Source, m_Offset + 1, m_Count - 1);
        }

        public byte[] Source { get; private set; }

        public byte Current
        {
            get { return Source[m_Offset]; }
        }

        public bool AtEnd
        {
            get { return m_Offset >= Source.Length || m_Count == 0; }
        }

        public int Position
        {
            get { return m_Offset; }
        }

        protected bool Equals(Input other)
        {
            return Equals(Source, other.Source) && m_Offset == other.m_Offset;
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
                return ((Source != null ? Source.GetHashCode() : 0)*397) ^ m_Offset;
            }
        }


    }
}