using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIOPL;
using GIOPL.Edison;
using GIOPL.SparkFunBoards.NDOF13033;

namespace GIOPL.SparkFun
{
    /*
        SparkFun 9 Degress of Freedom Board (DEV-13033 ROHS)

        ----------------------------------------------------
        Product URL: https://www.sparkfun.com/products/13033
        ----------------------------------------------------

        ----------------------------------------------------
        Code Base: https://github.com/sparkfun/SparkFun_9DOF_Block_for_Edison_CPP_Library
        ----------------------------------------------------

        -------------------
        Product Description
        -------------------

        The 9 Degrees of Freedom Block for the Intel® Edison uses the LSM9DS0 9DOF IMU for full-range motion sensing. 
        This chip combines a 3-axis accelerometer, a 3-axis gyroscope, and a 3-axis magnetometer. By default, the IMU is 
        connected to the Edison through the I2C bus. Each sensor in the LSM9DS0 supports a wide range of, well, ranges: 
        the accelerometer’s scale can be set to ± 2, 4, 6, 8, or 16 g, the gyroscope supports ± 245, 500, and 2000 °/s, 
        and the magnetometer has full-scale ranges of ± 2, 4, 8, or 12 gauss. Additionally, the LSM9DS0 includes an I2C 
        serial bus interface supporting standard and fast mode (100 kHz and 400 kHz) and an SPI serial standard interface.
         
    */
    public class NDOF13033 : IDisposable
    {
        public const byte WHO_AM_I_G = 0x0F;
        public const byte WHO_AM_I_XM = 0x0F;


        public byte GyroAddress { get; private set; }
        public byte AccelerometerAddress { get; private set; }
        public GyroConfig GyroConfig { get; set; } = new GyroConfig();
        public AccelConfig AccelConfig { get; set; } = new AccelConfig();
        private II2CDevice GyroI2c { get; set; }
        private II2CDevice AccelI2C { get; set; }

        private Vector3Data _lastestAccelData = new Vector3Data();
        public Vector3Data LatestAccelData
        {
            get
            {
                if (AccelDataAvalible)
                {
                    var newAccel = this.ReadAccel();

                    this._lastestAccelData.X = newAccel.X;
                    this._lastestAccelData.Y = newAccel.Y;
                    this._lastestAccelData.Z = newAccel.Z;
                }

                return this._lastestAccelData;
            }
        }

        private Vector3Data _lastestGyroData = new Vector3Data();
        public Vector3Data LatestGyroData
        {
            get
            {
                if (GyroDataAvalible)
                {
                    var newGyro = this.ReadGyro();

                    this._lastestGyroData.X = this.GyroConfig.GyroResolution * newGyro.X;
                    this._lastestGyroData.Y = this.GyroConfig.GyroResolution * newGyro.Y;
                    this._lastestGyroData.Z = this.GyroConfig.GyroResolution * newGyro.Z;
                }

                return this._lastestGyroData;
            }
        }

        public NDOF13033(II2CBusService i2cService, byte gyroAddress = 0x6B, byte accelerometerAddress = 0x1D)
        {
            GyroAddress = gyroAddress;
            AccelerometerAddress = accelerometerAddress;

            GyroI2c = i2cService.OpenDevice(gyroAddress);
            AccelI2C = i2cService.OpenDevice(accelerometerAddress);

            var gyroCheck = GyroI2c.ReadByte(NDOF13033.WHO_AM_I_G);
            var accelCheck = AccelI2C.ReadByte(NDOF13033.WHO_AM_I_XM);

            if (gyroCheck != 0xD4) throw new NDOF13033DeviceException("Gyro Failed Who_Am_I Test. Got: 0x" + gyroCheck.ToString("X2") + ", Expected: 0xD4");
            if (accelCheck != 0x49) throw new NDOF13033DeviceException("Accelerometer Failed Who_Am_I Test. Got: 0x" + accelCheck.ToString("X2") + ", Expected: 0x49");
        }

        public bool AccelDataAvalible
        {
            get
            {
                return ((0x8 & this.GyroI2c.ReadByte(DOFConsts.STATUS_REG_A)) != 0);
            }
        }

        public bool GyroDataAvalible
        {
            get
            {
                return (0x8 & this.GyroI2c.ReadByte(DOFConsts.STATUS_REG_G)) != 0;
            }
        }

        public void Init()
        {
            this.UpdateAccelConfig();
            this.UpdateGyroConfig();
        }

        public void UpdateAccelConfig()
        {
            this.AccelConfig.UpdateAccelConfig(this.AccelI2C);
        }
        public void UpdateGyroConfig()
        {
            this.GyroConfig.UpdateGyroConfig(this.GyroI2c);
        }

        private Vector3Data ReadSensorData(byte sensorAddress)
        {
            byte[] data = this.GyroI2c.ReadBytes(sensorAddress, 6);

            return new Vector3Data()
            {
                X = ((data[1] << 8) | data[0]),
                Y = ((data[3] << 8) | data[2]),
                Z = ((data[5] << 8) | data[4])
            };
        }

        public Vector3Data ReadAccel()
        {
            return this.ReadSensorData(DOFConsts.OUT_X_L_A);
        }

        public Vector3Data ReadGyro()
        {
            return this.ReadSensorData(DOFConsts.OUT_X_L_G);
        }

        public Vector3Data ReadMagnometer()
        {
            return this.ReadSensorData(DOFConsts.OUT_X_L_M);
        }

        public void Dispose()
        {
            this.GyroI2c.Close();
            this.AccelI2C.Close();
        }
    }
}
