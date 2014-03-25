using System;
using System.Net;
using System.Diagnostics;

/// <summary>
/// The ByteStream class serves as a thin wrapper around a byte[] array.  It's modeled roughly after the functionality
/// of a MemoryStream wrapped with a BinaryReader or BinaryWriter, but it handles endian conversions correctly,
/// and is ~3x faster.
/// </summary>
public class ByteStreamStruct
{
    #region Constructors
    public ByteStreamStruct()
    {
    }

    public ByteStreamStruct(byte[] array)
    {
        ByteArray = array;
        DataOffset = 0;
        CurrentOffset = 0;
        DataLength = array.Length;
    }

    public ByteStreamStruct(byte[] array, int offset, int length)
    {
        ByteArray = array;
        DataOffset = 0;
        DataLength = length;
        CurrentOffset = offset;
    }
    #endregion

    #region Fields and Properties

    public byte[] ByteArray;

    public int CurrentOffset;
    public int DataOffset;
    public int DataLength;

    #endregion

    #region Methods

    #region UInt16
    public ushort ReadUInt16Network()
    {
        ushort value = BitConverter.ToUInt16(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(System.BitConverter.ToUInt16(ByteArray, CurrentOffset))), 2);
        CurrentOffset += sizeof(ushort);
        return value;
    }

    public void WriteUInt16Network(ushort value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 2, ByteArray, CurrentOffset, sizeof(ushort));
        CurrentOffset += sizeof(ushort);
    }

    public ushort ReadUInt16()
    {
        ushort value = System.BitConverter.ToUInt16(ByteArray, CurrentOffset);
        CurrentOffset += sizeof(ushort);
        return value;
    }

    public void WriteUInt16(ushort value)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, ByteArray, CurrentOffset, sizeof(ushort));
        CurrentOffset += sizeof(ushort);
    }
    #endregion Uint16

    #region Int16
    public short ReadInt16Network()
    {
        short value = BitConverter.ToInt16(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(System.BitConverter.ToUInt16(ByteArray, CurrentOffset))), 2);
        CurrentOffset += sizeof(short);
        return value;
    }

    public void WriteInt16Network(short value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 2, ByteArray, CurrentOffset, sizeof(short));
        CurrentOffset += sizeof(short);
    }

    public short ReadInt16()
    {
        short value = System.BitConverter.ToInt16(ByteArray, CurrentOffset);
        CurrentOffset += sizeof(short);
        return value;
    }

    public void WriteInt16(short value)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, ByteArray, CurrentOffset, sizeof(short));
        CurrentOffset += sizeof(short);
    }
    #endregion Int16

    #region Int32
    public int ReadInt32Network()
    {
        int value = BitConverter.ToInt32(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(ByteArray, CurrentOffset))), 4);
        CurrentOffset += sizeof(int);
        return value;
    }

    public void WriteInt32Network(int value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 0, ByteArray, CurrentOffset, sizeof(int));
        CurrentOffset += sizeof(int);
    }

    public int ReadInt32()
    {
        int value = BitConverter.ToInt32(ByteArray, CurrentOffset);
        CurrentOffset += 4;
        return value;
    }

    public void WriteInt32(int value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 0, ByteArray, CurrentOffset, sizeof(int));
        CurrentOffset += sizeof(int);
    }
    #endregion Int32

    #region Uint32
    public uint ReadUInt32Network()
    {
        uint value = BitConverter.ToUInt32(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(System.BitConverter.ToUInt32(ByteArray, CurrentOffset))), 4);
        CurrentOffset += sizeof(uint);
        return value;
    }

    public void WriteUInt32Network(uint value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 4, ByteArray, CurrentOffset, sizeof(uint));
        CurrentOffset += sizeof(uint);
    }

    public uint ReadUInt32()
    {
        uint value = BitConverter.ToUInt32(ByteArray, CurrentOffset);
        CurrentOffset += sizeof(uint);
        return value;
    }

    public void WriteUInt32(uint value)
    {
        byte[] valueInBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        Buffer.BlockCopy(valueInBytes, 4, ByteArray, CurrentOffset, sizeof(uint));
        CurrentOffset += sizeof(uint);
    }
    #endregion Uint32

    #region Bytes
    public bool TryReadBytes(ByteStream destination, int length)
    {
        // Buffer.BlockCopy does the same checks, but it throws an exception when they fail, 
        // so it's faster to do the same check ourselves and then return false.
        if (CurrentOffset + length > ByteArray.Length || destination.CurrentOffset + length > destination.ByteArray.Length)
        {
            return false;
        }
        Buffer.BlockCopy(ByteArray, CurrentOffset, destination.ByteArray, destination.CurrentOffset, length);
        CurrentOffset += length;
        destination.CurrentOffset += length;
        return true;
    }

    public bool TryWriteBytes(ByteStream destination, int length)
    {
        if (CurrentOffset + length > ByteArray.Length || destination.CurrentOffset + length > destination.ByteArray.Length)
        {
            return false;
        }
        Buffer.BlockCopy(ByteArray, CurrentOffset, destination.ByteArray, destination.CurrentOffset, length);
        CurrentOffset += length;
        destination.CurrentOffset += length;
        return true;
    }

    public static byte ReadByte(byte[] packet, ref int offset)
    {
        return packet[offset++];
    }

    public static void WriteByte(byte value, byte[] destination, ref int offset)
    {
        destination[offset++] = value;
    }
    #endregion Bytes
    #endregion
}
