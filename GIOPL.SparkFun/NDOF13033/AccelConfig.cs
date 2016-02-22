using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.SparkFunBoards.NDOF13033
{
    public enum AccelDataRate : byte
    {
        PowerDown,
        Rate_3_125HZ,
        Rate_6_25HZ,
        Rate_12_5HZ,
        Rate_25HZ,
        Rate_50HZ,
        Rate_100HZ,
        Rate_200HZ,
        Rate_400HZ,
        Rate_800HZ,
        Rate_1600HZ
    }

    public enum AccelAntiAliasFilterBandwidth : byte
    {
        Bandwidth_773HZ,
        Bandwidth_194HZ,
        Bandwidth_362HZ,
        Bandwidth_50HZ
    }

    public enum AccelScaleSelection : byte
    {
        Scale_Plus_Minus_2g,
        Scale_Plus_Minus_4g,
        Scale_Plus_Minus_6g,
        Scale_Plus_Minus_8g,
        Scale_Plus_Minus_16g
    }

    public enum AccelSelfTest : byte
    {
        NoSelfTest,
        PositiveTest,
        NegativeTest
    }

    public class AccelConfig
    {
        /* Config For Register 0 */
        public bool RebootMemory { get; private set; } = false;
        public bool FifoEnabled { get; private set; } = false;
        public bool FifoWatermarkEnabled { get; private set; } = false;
        public bool HPFEnabledForClick { get; private set; } = false;
        public bool HPFEnabledGenerator1 { get; private set; } = false;
        public bool HPFEnabledGenerator2 { get; private set; } = false;

        /* Config For Register 1 */
        public AccelDataRate DataRate { get; private set; } = AccelDataRate.Rate_100HZ;
        /// <summary>
        /// Gets a value indicating whether to block data update for accel AND mag.
        /// </summary>
        /// <value>
        /// <c>true</c> if Continuous update; When <c>false</c> Output registers aren't updated until MSB and LSB have been read.
        /// </value>
        public bool BlockDataUpdateForAccelAndMag { get; private set; } = false;
        public bool ZAxisEnabled { get; private set; } = true;
        public bool YAxisEnabled { get; private set; } = true;
        public bool XAxisEnabled { get; private set; } = true;

        /* Config For Register 2 */
        public AccelAntiAliasFilterBandwidth AntiAliasFilterBandwidth { get; private set; } = AccelAntiAliasFilterBandwidth.Bandwidth_773HZ;
        public AccelScaleSelection Scale { get; private set; } = AccelScaleSelection.Scale_Plus_Minus_2g;
        public AccelSelfTest SelfTest { get; private set; } = AccelSelfTest.NoSelfTest;
        /// <summary>
        /// Gets a value indicating whether [sp i3 wire mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> If SPI 3 wire mode; otherwise, <c>false</c> indicates 4 write mode.
        /// </value>
        public bool SPI3WireMode { get; private set; } = false;




        public void UpdateAccelConfig(II2CDevice device) 
        {
            /* CTRL_REG0_XM (0x1F) (Default value: 0x00)
            Bits (7-0): BOOT FIFO_EN WTM_EN 0 0 HP_CLICK HPIS1 HPIS2
            BOOT - Reboot memory content (0: normal, 1: reboot)
            FIFO_EN - Fifo enable (0: disable, 1: enable)
            WTM_EN - FIFO watermark enable (0: disable, 1: enable)
            HP_CLICK - HPF enabled for click (0: filter bypassed, 1: enabled)
            HPIS1 - HPF enabled for interrupt generator 1 (0: bypassed, 1: enabled)
            HPIS2 - HPF enabled for interrupt generator 2 (0: bypassed, 1 enabled)   */
            byte reg0Config = (byte)((HPFEnabledGenerator2 ? 1 : 0) << 7 &
                                     (HPFEnabledGenerator1 ? 1 : 0) << 6 &
                                     (HPFEnabledForClick ? 1 : 0) << 5 &
                                     (FifoWatermarkEnabled ? 1 : 0) << 2 &
                                     (FifoEnabled ? 1 : 0) << 1 &
                                     (RebootMemory ? 1 : 0));

            /* CTRL_REG1_XM (0x20) (Default value: 0x07)
            Bits (7-0): AODR3 AODR2 AODR1 AODR0 BDU AZEN AYEN AXEN
            AODR[3:0] - select the acceleration data rate:
                0000=power down, 0001=3.125Hz, 0010=6.25Hz, 0011=12.5Hz, 
                0100=25Hz, 0101=50Hz, 0110=100Hz, 0111=200Hz, 1000=400Hz,
                1001=800Hz, 1010=1600Hz, (remaining combinations undefined).
            BDU - block data update afor accel AND mag
                0: Continuous update
                1: Output registers aren't updated until MSB and LSB have been read.
            AZEN, AYEN, and AXEN - Acceleration x/y/z-axis enabled.
                0: Axis disabled, 1: Axis enabled									 */
            byte reg1Config = (byte)((XAxisEnabled ? 1 : 0) << 7 &
                                     (YAxisEnabled ? 1 : 0) << 6 &
                                     (ZAxisEnabled ? 1 : 0) << 5 &
                                     (BlockDataUpdateForAccelAndMag ? 1 : 0) << 4 &
                                     (int) DataRate);

            /* CTRL_REG2_XM (0x21) (Default value: 0x00)
            Bits (7-0): ABW1 ABW0 AFS2 AFS1 AFS0 AST1 AST0 SIM
            ABW[1:0] - Accelerometer anti-alias filter bandwidth
                00=773Hz, 01=194Hz, 10=362Hz, 11=50Hz
            AFS[2:0] - Accel full-scale selection
                000=+/-2g, 001=+/-4g, 010=+/-6g, 011=+/-8g, 100=+/-16g
            AST[1:0] - Accel self-test enable
                00=normal (no self-test), 01=positive st, 10=negative st, 11=not allowed
            SIM - SPI mode selection
                0=4-wire, 1=3-wire													 */
            byte reg2Config = (byte)((SPI3WireMode ? 1 : 0) << 7 &
                                     (int)SelfTest << 5 &
                                     (int)Scale << 2 &
                                     (int)AntiAliasFilterBandwidth);




            device.WriteByte(DOFConsts.CTRL_REG0_XM, reg0Config);
            device.WriteByte(DOFConsts.CTRL_REG1_XM, reg1Config);
            device.WriteByte(DOFConsts.CTRL_REG2_XM, reg2Config);

            // NOTE: Not sure how this register should be set documentation says:
            /* CTRL_REG3_XM is used to set interrupt generators on INT1_XM
               Bits (7-0): P1_BOOT P1_TAP P1_INT1 P1_INT2 P1_INTM P1_DRDYA P1_DRDYM P1_EMPTY
            */
            // Accelerometer data ready on INT1_XM (0x04)
            device.WriteByte(DOFConsts.CTRL_REG3_XM, 0x04);
        }

    }
}
