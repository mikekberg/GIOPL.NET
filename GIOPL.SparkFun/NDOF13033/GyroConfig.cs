using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.SparkFunBoards.NDOF13033
{
    // TODO: Breakup all of these like in AccelConfig.cs

    public class GyroConfig
    {
        public const byte CTRL_REG1_G = 0x20;
        public const byte CTRL_REG2_G = 0x21;
        public const byte CTRL_REG3_G = 0x22;
        public const byte CTRL_REG4_G = 0x23;
        public const byte CTRL_REG5_G = 0x24;

        /* CTRL_REG1_G sets output data rate, bandwidth, power-down and enables
        Bits[7:0]: DR1 DR0 BW1 BW0 PD Zen Xen Yen
        DR[1:0] - Output data rate selection
            00=95Hz, 01=190Hz, 10=380Hz, 11=760Hz
        BW[1:0] - Bandwidth selection (sets cutoff frequency)
             Value depends on ODR. See datasheet table 21.
        PD - Power down enable (0=power down mode, 1=normal or sleep mode)
        Zen, Xen, Yen - Axis enable (o=disabled, 1=enabled)	*/
        public byte OutputDataRate = 0x0F;

        /* CTRL_REG2_G sets up the HPF
        Bits[7:0]: 0 0 HPM1 HPM0 HPCF3 HPCF2 HPCF1 HPCF0
        HPM[1:0] - High pass filter mode selection
            00=normal (reset reading HP_RESET_FILTER, 01=ref signal for filtering,
            10=normal, 11=autoreset on interrupt
        HPCF[3:0] - High pass filter cutoff frequency
            Value depends on data rate. See datasheet table 26. */
        public byte HighPassFilter = 0x00;

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
        public byte InteruptDRDY = 0x88;

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
        public byte ScaleAndUpdate = 0x00;

        /* CTRL_REG5_G sets up the FIFO, HPF, and INT1
        Bits[7:0] - BOOT FIFO_EN - HPen INT1_Sel1 INT1_Sel0 Out_Sel1 Out_Sel0
        BOOT - Reboot memory content (0=normal, 1=reboot)
        FIFO_EN - FIFO enable (0=disable, 1=enable)
        HPen - HPF enable (0=disable, 1=enable)
        INT1_Sel[1:0] - Int 1 selection configuration
        Out_Sel[1:0] - Out selection configuration */
        public byte FIFOHPFINT1 = 0x00;

        private GyroScale _scale = GyroScale.G_SCALE_245DPS;
        public double GyroResolution
        {
            get
            {
                switch (_scale)
                {
                    case GyroScale.G_SCALE_245DPS:
                        return 245.0 / 32768.0;
                    case GyroScale.G_SCALE_500DPS:
                        return 500.0 / 32768.0;
                    case GyroScale.G_SCALE_2000DPS:
                        return 2000.0 / 32768.0;
                }

                return 0;
            }
        }

        public void UpdateGyroConfig(II2CDevice device)
        {
            device.WriteByte(CTRL_REG1_G, this.OutputDataRate);
            device.WriteByte(CTRL_REG2_G, this.HighPassFilter);
            device.WriteByte(CTRL_REG3_G, this.InteruptDRDY);
            device.WriteByte(CTRL_REG4_G, this.ScaleAndUpdate);
            device.WriteByte(CTRL_REG5_G, this.FIFOHPFINT1);
        }
    }
}
