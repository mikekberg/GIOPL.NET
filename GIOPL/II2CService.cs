using System;

namespace GIOPL
{
    public interface II2CBusService
    {
        int Bus { get; }
        II2CDevice OpenDevice(byte address, I2CModes mode = I2CModes.Standard);
    }

    public interface II2CDevice : IDisposable
    {
        byte Address { get; }
        I2CModes Frequency { get; }
        void Close();
        int Read(byte[] buffer);
        byte ReadByte();
        byte ReadByte(byte subaddress);
        byte[] ReadBytes(byte subaddress, int length);
        void Write(byte[] buffer);
        void WriteByte(byte value);
        void WriteByte(byte subAddress, byte data);
    }
}