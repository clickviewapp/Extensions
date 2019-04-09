namespace ClickView.Extensions.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /*
     * All buffer allocation is intentional for performance. Please don't change to a loop
     */
    public class ByteWriter
    {
        private readonly byte[] _bytes;
        private int _position;

        public ByteWriter(byte[] bytes) : this(bytes, 0)
        {
        }

        public ByteWriter(byte[] bytes, int offset)
        {
            _bytes = bytes;
            _position = offset;
        }

        public int Seek(int count, SeekOrigin seekOrigin)
        {
            switch (seekOrigin)
            {
                case SeekOrigin.Begin:
                    _position = count;
                    break;
                case SeekOrigin.Current:
                    _position += count;
                    break;
                case SeekOrigin.End:
                    _position = _bytes.Length - count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seekOrigin), seekOrigin, null);
            }

            return _position;
        }

        public void Write(ushort value)
        {
            Write2(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write4(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            Write4(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            Write8(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write8(BitConverter.GetBytes(value));
        }

        public void Write(byte[] value)
        {
            Buffer.BlockCopy(value, 0, _bytes, _position, value.Length);

            _position += value.Length;
        }

        public void Write(Guid value)
        {
            Write(value.ToByteArray());
        }

        public void Write(byte value)
        {
            _bytes[_position] = value;

            ++_position;
        }

        private void Write2(IReadOnlyList<byte> b)
        {
            _bytes[_position + 0] = b[0];
            _bytes[_position + 1] = b[1];

            _position += 2;
        }

        private void Write4(IReadOnlyList<byte> b)
        {
            _bytes[_position + 0] = b[0];
            _bytes[_position + 1] = b[1];
            _bytes[_position + 2] = b[2];
            _bytes[_position + 3] = b[3];

            _position += 4;
        }

        private void Write8(IReadOnlyList<byte> b)
        {
            _bytes[_position + 0] = b[0];
            _bytes[_position + 1] = b[1];
            _bytes[_position + 2] = b[2];
            _bytes[_position + 3] = b[3];
            _bytes[_position + 4] = b[4];
            _bytes[_position + 5] = b[5];
            _bytes[_position + 6] = b[6];
            _bytes[_position + 7] = b[7];

            _position += 8;
        }
    }
}
