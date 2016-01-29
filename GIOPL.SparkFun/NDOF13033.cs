using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIOPL;
using GIOPL.Edison;

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
        public byte GyroAddress { get; private set; }
        public byte AccelerometerAddress { get; private set; }
        public gyro_config GyroConfig { get; set; }
        private II2CDevice GyroI2c { get; set; }
        private II2CDevice AccelI2C { get; set; }

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

            // Set Config Defaults
            this.GyroConfig = new gyro_config(0x0F, 0x00, 0x88, 0x00, 0x00);

            GyroI2c = i2cService.OpenDevice(gyroAddress);
            AccelI2C = i2cService.OpenDevice(accelerometerAddress);

            var gyroCheck = GyroI2c.ReadByte(DOFConsts.WHO_AM_I_G);
            var accelCheck = AccelI2C.ReadByte(DOFConsts.WHO_AM_I_XM);

            if (gyroCheck != 0xD4) throw new NDOF13033DeviceException("Gyro Failed Who_Am_I Test. Got: 0x" + gyroCheck.ToString("X2") + ", Expected: 0xD4");
            if (accelCheck != 0x49) throw new NDOF13033DeviceException("Accelerometer Failed Who_Am_I Test. Got: 0x" + accelCheck.ToString("X2") + ", Expected: 0x49");
        }

        public void InitGyro()
        {
            GyroI2c.WriteByte(DOFConsts.CTRL_REG1_G, GyroConfig.OutputDataRate);
            GyroI2c.WriteByte(DOFConsts.CTRL_REG2_G, GyroConfig.HighPassFilter);
            GyroI2c.WriteByte(DOFConsts.CTRL_REG3_G, GyroConfig.InteruptDRDY);
            GyroI2c.WriteByte(DOFConsts.CTRL_REG4_G, GyroConfig.Scale);
            GyroI2c.WriteByte(DOFConsts.CTRL_REG5_G, GyroConfig.FIFOHPFINT1);
        }

        public GyroData ReadGyro()
        {
            byte[] data = GyroI2c.ReadBytes(DOFConsts.OUT_X_L_G);


            return new GyroData()
            {
                gx = ((data[1] << 8) | data[0]),
                gy = ((data[3] << 8) | data[2]),
                gz = ((data[5] << 8) | data[4])
            };
        }

        public void Dispose()
        {
            GyroI2c.Close();
            AccelI2C.Close();
        }

        public class NDOF13033DeviceException : Exception
        {
            public NDOF13033DeviceException(string message) : base(message) { }
        }

        public struct GyroData
        {
            public int gx;
            public int gy;
            public int gz;
        }

        private static class DOFConsts
        {
            ////////////////////////////
            // LSM9DS0 Gyro Registers //
            ////////////////////////////
            public const byte WHO_AM_I_G = 0x0F;
            public const byte CTRL_REG1_G = 0x20;
            public const byte CTRL_REG2_G = 0x21;
            public const byte CTRL_REG3_G = 0x22;
            public const byte CTRL_REG4_G = 0x23;
            public const byte CTRL_REG5_G = 0x24;
            public const byte REFERENCE_G = 0x25;
            public const byte STATUS_REG_G = 0x27;
            public const byte OUT_X_L_G = 0x28;
            public const byte OUT_X_H_G = 0x29;
            public const byte OUT_Y_L_G = 0x2A;
            public const byte OUT_Y_H_G = 0x2B;
            public const byte OUT_Z_L_G = 0x2C;
            public const byte OUT_Z_H_G = 0x2D;
            public const byte FIFO_CTRL_REG_G = 0x2E;
            public const byte FIFO_SRC_REG_G = 0x2F;
            public const byte INT1_CFG_G = 0x30;
            public const byte INT1_SRC_G = 0x31;
            public const byte INT1_THS_XH_G = 0x32;
            public const byte INT1_THS_XL_G = 0x33;
            public const byte INT1_THS_YH_G = 0x34;
            public const byte INT1_THS_YL_G = 0x35;
            public const byte INT1_THS_ZH_G = 0x36;
            public const byte INT1_THS_ZL_G = 0x37;
            public const byte INT1_DURATION_G = 0x38;

            //////////////////////////////////////////
            // LSM9DS0 Accel/Magneto (XM) Registers //
            //////////////////////////////////////////
            public const byte OUT_TEMP_L_XM = 0x05;
            public const byte OUT_TEMP_H_XM = 0x06;
            public const byte STATUS_REG_M = 0x07;
            public const byte OUT_X_L_M = 0x08;
            public const byte OUT_X_H_M = 0x09;
            public const byte OUT_Y_L_M = 0x0A;
            public const byte OUT_Y_H_M = 0x0B;
            public const byte OUT_Z_L_M = 0x0C;
            public const byte OUT_Z_H_M = 0x0D;
            public const byte WHO_AM_I_XM = 0x0F;
            public const byte INT_CTRL_REG_M = 0x12;
            public const byte INT_SRC_REG_M = 0x13;
            public const byte INT_THS_L_M = 0x14;
            public const byte INT_THS_H_M = 0x15;
            public const byte OFFSET_X_L_M = 0x16;
            public const byte OFFSET_X_H_M = 0x17;
            public const byte OFFSET_Y_L_M = 0x18;
            public const byte OFFSET_Y_H_M = 0x19;
            public const byte OFFSET_Z_L_M = 0x1A;
            public const byte OFFSET_Z_H_M = 0x1B;
            public const byte REFERENCE_X = 0x1C;
            public const byte REFERENCE_Y = 0x1D;
            public const byte REFERENCE_Z = 0x1E;
            public const byte CTRL_REG0_XM = 0x1F;
            public const byte CTRL_REG1_XM = 0x20;
            public const byte CTRL_REG2_XM = 0x21;
            public const byte CTRL_REG3_XM = 0x22;
            public const byte CTRL_REG4_XM = 0x23;
            public const byte CTRL_REG5_XM = 0x24;
            public const byte CTRL_REG6_XM = 0x25;
            public const byte CTRL_REG7_XM = 0x26;
            public const byte STATUS_REG_A = 0x27;
            public const byte OUT_X_L_A = 0x28;
            public const byte OUT_X_H_A = 0x29;
            public const byte OUT_Y_L_A = 0x2A;
            public const byte OUT_Y_H_A = 0x2B;
            public const byte OUT_Z_L_A = 0x2C;
            public const byte OUT_Z_H_A = 0x2D;
            public const byte FIFO_CTRL_REG = 0x2E;
            public const byte FIFO_SRC_REG = 0x2F;
            public const byte INT_GEN_1_REG = 0x30;
            public const byte INT_GEN_1_SRC = 0x31;
            public const byte INT_GEN_1_THS = 0x32;
            public const byte INT_GEN_1_DURATION = 0x33;
            public const byte INT_GEN_2_REG = 0x34;
            public const byte INT_GEN_2_SRC = 0x35;
            public const byte INT_GEN_2_THS = 0x36;
            public const byte INT_GEN_2_DURATION = 0x37;
            public const byte CLICK_CFG = 0x38;
            public const byte CLICK_SRC = 0x39;
            public const byte CLICK_THS = 0x3A;
            public const byte TIME_LIMIT = 0x3B;
            public const byte TIME_LATENCY = 0x3C;
            public const byte TIME_WINDOW = 0x3D;
            public const byte ACT_THS = 0x3E;
            public const byte ACT_DUR = 0x3F;
        }
        public enum gyro_odr : byte
        {                           // ODR (Hz) --- Cutoff
            G_ODR_95_BW_125 = 0x0, //   95         12.5
            G_ODR_95_BW_25 = 0x1, //   95          25
                                  // 0x2 and 0x3 define the same data rate and bandwidth
            G_ODR_190_BW_125 = 0x4, //   190        12.5
            G_ODR_190_BW_25 = 0x5, //   190         25
            G_ODR_190_BW_50 = 0x6, //   190         50
            G_ODR_190_BW_70 = 0x7, //   190         70
            G_ODR_380_BW_20 = 0x8, //   380         20
            G_ODR_380_BW_25 = 0x9, //   380         25
            G_ODR_380_BW_50 = 0xA, //   380         50
            G_ODR_380_BW_100 = 0xB, //   380         100
            G_ODR_760_BW_30 = 0xC, //   760         30
            G_ODR_760_BW_35 = 0xD, //   760         35
            G_ODR_760_BW_50 = 0xE, //   760         50
            G_ODR_760_BW_100 = 0xF, //   760         100
        };

        public enum gyro_scale : byte
        {
            G_SCALE_245DPS,     // 00:  245 degrees per second
            G_SCALE_500DPS,     // 01:  500 dps
            G_SCALE_2000DPS,    // 10:  2000 dps
        };

        public struct gyro_config
        {
            /* CTRL_REG1_G sets output data rate, bandwidth, power-down and enables
            Bits[7:0]: DR1 DR0 BW1 BW0 PD Zen Xen Yen
            DR[1:0] - Output data rate selection
                00=95Hz, 01=190Hz, 10=380Hz, 11=760Hz
            BW[1:0] - Bandwidth selection (sets cutoff frequency)
                 Value depends on ODR. See datasheet table 21.
            PD - Power down enable (0=power down mode, 1=normal or sleep mode)
            Zen, Xen, Yen - Axis enable (o=disabled, 1=enabled)	*/
            public byte OutputDataRate;

            /* CTRL_REG2_G sets up the HPF
            Bits[7:0]: 0 0 HPM1 HPM0 HPCF3 HPCF2 HPCF1 HPCF0
            HPM[1:0] - High pass filter mode selection
                00=normal (reset reading HP_RESET_FILTER, 01=ref signal for filtering,
                10=normal, 11=autoreset on interrupt
            HPCF[3:0] - High pass filter cutoff frequency
                Value depends on data rate. See datasheet table 26. */
            public byte HighPassFilter;

            /* CTRL_REG3_G sets up interrupt and DRDY_G pins
            Bits[7:0]: I1_IINT1 I1_BOOT H_LACTIVE PP_OD I2_DRDY I2_WTM I2_ORUN I2_EMPTY
            I1_INT1 - Interrupt enable on INT_G pin (0=disable, 1=enable)
            I1_BOOT - Boot status available on INT_G (0=disable, 1=enable)
            H_LACTIVE - Interrupt active configuration on INT_G (0:high, 1:low)
            PP_OD - Push-pull/open-drain (0=push-pull, 1=open-drain)
            I2_DRDY - Data ready on DRDY_G (0=disable, 1=enable)
            I2_WTM - FIFO watermark interrupt on DRDY_G (0=disable 1=enable)
            I2_ORUN - FIFO overrun interrupt on DRDY_G (0=disable 1=enable)
            I2_EMPTY - FIFO empty interrupt on DRDY_G (0=disable 1=enable) */
            // Int1 enabled (pp, active low), data read on DRDY_G:
            public byte InteruptDRDY;

            /* CTRL_REG4_G sets the scale, update mode
            Bits[7:0] - BDU BLE FS1 FS0 - ST1 ST0 SIM
            BDU - Block data update (0=continuous, 1=output not updated until read
            BLE - Big/little endian (0=data LSB @ lower address, 1=LSB @ higher add)
            FS[1:0] - Full-scale selection
                00=245dps, 01=500dps, 10=2000dps, 11=2000dps
            ST[1:0] - Self-test enable
                00=disabled, 01=st 0 (x+, y-, z-), 10=undefined, 11=st 1 (x-, y+, z+)
            SIM - SPI serial interface mode select
                0=4 wire, 1=3 wire */
            public byte Scale;

            /* CTRL_REG5_G sets up the FIFO, HPF, and INT1
            Bits[7:0] - BOOT FIFO_EN - HPen INT1_Sel1 INT1_Sel0 Out_Sel1 Out_Sel0
            BOOT - Reboot memory content (0=normal, 1=reboot)
            FIFO_EN - FIFO enable (0=disable, 1=enable)
            HPen - HPF enable (0=disable, 1=enable)
            INT1_Sel[1:0] - Int 1 selection configuration
            Out_Sel[1:0] - Out selection configuration */
            public byte FIFOHPFINT1;


            public gyro_config(byte outputDataRate = 0x0F, byte highPassFilter = 0x00, byte interuptDRDY = 0x88, byte scale = 0x00, byte fifohpfint1 = 0x00)
            {
                OutputDataRate = outputDataRate;
                HighPassFilter = highPassFilter;
                InteruptDRDY = interuptDRDY;
                Scale = scale;
                FIFOHPFINT1 = fifohpfint1;
            }
        }
    }
}
