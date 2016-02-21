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
        private II2CDevice GyroI2c { get; set; }
        private II2CDevice AccelI2C { get; set; }

        private Vector3Data _lastestAccelData = new Vector3Data();
        public Vector3Data LatestAccelData
        {
            get
            {
                if (AccelDataAvalible)
                {
                    var newAccel = ReadAccel();

                    _lastestAccelData.X = newAccel.X;
                    _lastestAccelData.Y = newAccel.Y;
                    _lastestAccelData.Z = newAccel.Z;
                }

                return _lastestAccelData;
            }
        }

        private Vector3Data _lastestGyroData = new Vector3Data();
        public Vector3Data LatestGyroData
        {
            get
            {
                if (GyroDataAvalible)
                {
                    var newGyro = ReadGyro();

                    _lastestGyroData.X = GyroConfig.GyroResolution * newGyro.X;
                    _lastestGyroData.Y = GyroConfig.GyroResolution * newGyro.Y;
                    _lastestGyroData.Z = GyroConfig.GyroResolution * newGyro.Z;
                }

                return _lastestGyroData;
            }
        }

        public bool AccelDataAvalible
        {
            get
            {
                return ((0x8 & GyroI2c.ReadByte(DOFConsts.STATUS_REG_A)) != 0);
            }
        }

        public bool GyroDataAvalible
        {
            get
            {
                return (0x8 & GyroI2c.ReadByte(DOFConsts.STATUS_REG_G)) != 0;
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

        public void Init()
        {
            GyroConfig.InitGyro(this.GyroI2c);
        }

        private Vector3Data ReadSensorData(byte sensorAddress)
        {
            byte[] data = GyroI2c.ReadBytes(sensorAddress, 6);

            return new Vector3Data()
            {
                X = ((data[1] << 8) | data[0]),
                Y = ((data[3] << 8) | data[2]),
                Z = ((data[5] << 8) | data[4])
            };
        }

        public Vector3Data ReadAccel()
        {
            return ReadSensorData(DOFConsts.OUT_X_L_A);
        }

        public Vector3Data ReadGyro()
        {
            return ReadSensorData(DOFConsts.OUT_X_L_G);
        }

        public Vector3Data ReadMagnometer()
        {
            return ReadSensorData(DOFConsts.OUT_X_L_M);
        }

        public void Dispose()
        {
            GyroI2c.Close();
            AccelI2C.Close();
        }
    }
}
