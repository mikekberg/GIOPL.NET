using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.Edison
{
    using System;
    using System.Runtime.InteropServices;


    public class EdisonI2C : II2CBusService
    {
        public EdisonI2C(int bus)
        {
            this.Bus = bus;

            MRAAInterop.Initialize();
        }

        public int Bus { get; private set; }

        public II2CDevice OpenDevice(byte address, I2CModes mode = I2CModes.Standard)
        {
            IntPtr dev = MRAAInterop.mraa_i2c_init(this.Bus);

            if (dev == IntPtr.Zero)
                throw new MRAAException(MRAAInterop.Result.ErrorUnspecified);

            return new EdisonI2CDevice(dev, address, mode);
        }
    }

    public class EdisonI2CDevice : II2CDevice
    {
        #region Variables
        private IntPtr device = IntPtr.Zero;
        #endregion

        #region Accessors
        public I2CModes Frequency { get; private set; }
        public byte Address { get; private set; }

        byte II2CDevice.Address
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        I2CModes II2CDevice.Frequency
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Constructors and Initialization
        internal EdisonI2CDevice(IntPtr dev, byte address, I2CModes frequency)
        {
            this.device = dev;
            this.Address = address;
            this.Frequency = frequency;

            var freqResult = MRAAInterop.mraa_i2c_frequency(this.device, (byte)frequency);
            if (freqResult != MRAAInterop.Result.Success)
                throw new MRAAException(freqResult);

            var addressResult = MRAAInterop.mraa_i2c_address(this.device, (byte)address);
            if (addressResult != MRAAInterop.Result.Success)
                throw new MRAAException(addressResult);
        }
        #endregion

        #region Read
        public int Read(byte[] buffer)
        {
            return MRAAInterop.mraa_i2c_read(this.device, buffer, buffer.Length);
        }

        public byte ReadByte()
        {
            return MRAAInterop.mraa_i2c_read_byte(this.device);
        }

        public byte ReadByte(byte subaddress)
        {
            return MRAAInterop.mraa_i2c_read_byte_data(this.device, subaddress);
        }
        public byte[] ReadBytes(byte subaddress, int length)
        {
            byte[] data = new byte[length];

            MRAAInterop.mraa_i2c_read_bytes_data(this.device, subaddress, data, length);

            return data;
        }
        #endregion

        #region Write
        public void Write(byte[] buffer)
        {
            var result = MRAAInterop.mraa_i2c_write(this.device, buffer, buffer.Length);
            if (result != MRAAInterop.Result.Success)
                throw new MRAAException(result);
        }

        public void WriteByte(byte value)
        {
            var result = MRAAInterop.mraa_i2c_write_byte(this.device, value);
            if (result != MRAAInterop.Result.Success)
                throw new MRAAException(result);
        }

        public void WriteByte(byte preData, byte data)
        {
            var result = MRAAInterop.mraa_i2c_write_byte_data(this.device, data, preData);
            if (result != MRAAInterop.Result.Success)
                throw new MRAAException(result);
        }
        #endregion

        #region Cleanup
        public void Close()
        {
            if (this.device == IntPtr.Zero)
                return;
            MRAAInterop.mraa_i2c_stop(this.device);
            this.device = IntPtr.Zero;
        }

        public void Dispose()
        {
            if (this.device != IntPtr.Zero)
                this.Close();
        }
        #endregion
    }
}
