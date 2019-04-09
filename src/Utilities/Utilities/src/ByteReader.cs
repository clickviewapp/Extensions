namespace ClickView.Extensions.Utilities
{
    using System;
    using System.IO;

    public class ByteReader
    {
        private readonly byte[] _bytes;
        public int Position { get; private set; }

        public ByteReader(byte[] bytes)
        {
            _bytes = bytes;
            Position = 0;
        }

        public int Seek(int count, SeekOrigin seekOrigin)
        {
            switch (seekOrigin)
            {
                case SeekOrigin.Begin:
                    Position = count;
                    break;
                case SeekOrigin.Current:
                    Position += count;
                    break;
                case SeekOrigin.End:
                    Position = _bytes.Length - count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seekOrigin), seekOrigin, null);
            }

            return Position;
        }

        public ulong ReadUInt64()
        {
            Position += 8;
            return BitConverter.ToUInt64(_bytes, Position - 8);
        }

        public long ReadInt64()
        {
            Position += 8;
            return BitConverter.ToInt64(_bytes, Position - 8);
        }

        public uint ReadUInt32()
        {
            Position += 4;
            return BitConverter.ToUInt32(_bytes, Position - 4);
        }

        public int ReadInt32()
        {
            Position += 4;
            return BitConverter.ToInt32(_bytes, Position - 4);
        }

        public ushort ReadUInt16()
        {
            Position += 2;
            return BitConverter.ToUInt16(_bytes, Position - 2);
        }

        public byte ReadByte()
        {
            return _bytes[Position++];
        }

        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];

            Buffer.BlockCopy(_bytes, Position, buffer, 0, count);
            Position += count;

            return buffer;
        }

        public byte[] ReadToEnd()
        {
            var left = _bytes.Length - Position;

            return ReadBytes(left);
        }

        public Guid ReadGuid()
        {
            return new Guid(ReadBytes(16));
        }
    }
}
