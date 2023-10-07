﻿using System.Text;

namespace qASIC.Communication
{
    public class Packet
    {
        public Packet() : this(new byte[0]) { }

        public Packet(byte[] bytes)
        {
            this.bytes = new List<byte>(bytes);
        }

        public List<byte> bytes;
        public int position;

        public byte[] ToArray() => bytes.ToArray();

        public byte[] ReadCurrentBytes(int length)
        {
            byte[] currentBytes = bytes.Count - position >= length?
                bytes.GetRange(position, length).ToArray() :
                new byte[length];

            position += length;
            return currentBytes;
        }

        public bool ReadBool() => BitConverter.ToBoolean(ReadCurrentBytes(1), 0);
        public byte ReadByte() => ReadCurrentBytes(1).First();
        public sbyte ReadSByte() => unchecked((sbyte)ReadCurrentBytes(1).FirstOrDefault((byte)0));
        public int ReadInt() => BitConverter.ToInt32(ReadCurrentBytes(sizeof(int)), 0);
        public uint ReadUInt() => BitConverter.ToUInt32(ReadCurrentBytes(sizeof(uint)), 0);
        public float ReadFloat() => BitConverter.ToSingle(ReadCurrentBytes(sizeof(float)), 0);
        public double ReadDouble() => BitConverter.ToDouble(ReadCurrentBytes(sizeof(double)), 0);
        public long ReadLong() => BitConverter.ToInt64(ReadCurrentBytes(sizeof(long)), 0);
        public ulong ReadULong() => BitConverter.ToUInt64(ReadCurrentBytes(sizeof(ulong)), 0);
        public string ReadString() => Encoding.UTF8.GetString(ReadCurrentBytes(ReadInt()));

        public T ReadNetworkSerializable<T>() where T : INetworkSerializable, new()
        {
            var item = new T();
            item.Read(this);
            return item;
        }

        public Packet WriteBytes(params byte[] data)
        {
            bytes.AddRange(data);
            return this;
        }

        public Packet Write(bool value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(byte value) => WriteBytes(value);
        public Packet Write(sbyte value) => WriteBytes(unchecked((byte)value));
        public Packet Write(int value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(uint value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(float value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(double value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(long value) => WriteBytes(BitConverter.GetBytes(value));
        public Packet Write(ulong value) => WriteBytes(BitConverter.GetBytes(value));

        public Packet Write(string value)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(value);
            Write(textBytes.Length);
            return WriteBytes(textBytes);
        }

        public Packet Write(INetworkSerializable item) =>
            item.Write(this);

        public override string ToString() =>
            $"Packet length:{bytes.Count} position:{position} bytes:{string.Join(",", bytes)}";
    }
}