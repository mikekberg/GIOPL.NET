using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.Edison
{
    public class MRAAInterop
    {
        #region Enumerators
        public enum Result : int
        {
            Success = 0, FeatureNotImplemented = 1,
            FeatureNotSupported = 2, InvalidVerbosityLevel = 3,
            InvalidParameter = 4, InvalidHandle = 5,
            NoResources = 6, InvalidResource = 7,
            InvalidQueueType = 8, NoDataAvaliable = 9,
            InvalidPlatform = 10, PlatformNotInitialized = 11,
            PlatformAlreadyInitialized = 12, ErrorUnspecified = 99
        }
        public enum Platform : int
        {
            IntelGalileoGen1 = 0, IntelGalileoGen2 = 1, IntelEdisonFabC = 2,
            IntelDE3815 = 3, IntelMinnowBoardMax = 4, RaspberryPi = 5,
            BeagleBone = 6, Banana = 7, Unknown
        }
        #endregion

        #region Accessors
        public static string PlatformName
        {
            get
            {
                return Marshal.PtrToStringAuto(mraa_get_platform_name());
            }
        }
        public static string Version
        {
            get
            {
                return Marshal.PtrToStringAuto(mraa_get_version());
            }
        }
        public static Platform PlatformType
        {
            get
            {
                return mraa_get_platform_type();
            }
        }
        #endregion

        private static bool MRAAInit = false;

        #region Constructors and Initialization
        /// <summary>
        /// Initialize MRAA library
        /// </summary>
        /// <returns>Success on first initialization, 
        /// PlatformAlreadyInitialized if MRAA 
        /// has been initialized previously</returns>
        public static Result Initialize()
        {
            if (!MRAAInit)
            {
                MRAAInit = true;
                return mraa_init();
            }

            return Result.Success;
        }
        #endregion

        #region Deinitialization
        /// <summary>
        /// MRAA documentation states that it is not nessesary to call this function normally 
        /// unless you intend to dynamically unload this library.
        /// </summary>
        public static void Deinitialize()
        {
            mraa_deinit();
        }
        #endregion

        #region External Functions
        [DllImport("libmraa.so")]
        private static extern Result mraa_init();

        [DllImport("libmraa.so")]
        private static extern void mraa_deinit();

        [DllImport("libmraa.so")]
        private static extern IntPtr mraa_get_platform_name();

        [DllImport("libmraa.so")]
        private static extern IntPtr mraa_get_version();

        [DllImport("libmraa.so")]
        private static extern Platform mraa_get_platform_type();
        #endregion

        #region External Functions
        [DllImport("libmraa.so")]
        public static extern IntPtr mraa_i2c_init(int bus);

        [DllImport("libmraa.so")]
        public static extern void mraa_i2c_stop(IntPtr device);

        [DllImport("libmraa.so")]
        public static extern MRAAInterop.Result mraa_i2c_frequency(IntPtr device, int mode);

        [DllImport("libmraa.so")]
        public static extern MRAAInterop.Result mraa_i2c_address(IntPtr device, byte address);

        [DllImport("libmraa.so")]
        public static extern int mraa_i2c_read(IntPtr device, byte[] data, int length);

        [DllImport("libmraa.so")]
        public static extern byte mraa_i2c_read_byte(IntPtr device);

        [DllImport("libmraa.so")]
        public static extern MRAAInterop.Result mraa_i2c_write(IntPtr device, byte[] data, int length);

        [DllImport("libmraa.so")]
        public static extern MRAAInterop.Result mraa_i2c_write_byte_data(IntPtr device, byte subaddress, byte command);

        [DllImport("libmraa.so")]
        public static extern byte mraa_i2c_read_byte_data(IntPtr device, byte subaddress);

        [DllImport("libmraa.so")]
        public static extern MRAAInterop.Result mraa_i2c_write_byte(IntPtr device, byte value);

        [DllImport("libmraa.so")]
        public static extern int mraa_i2c_read_bytes_data(IntPtr device, byte command, byte[] data, int length);
        #endregion
    }

    public class MRAAException : Exception
    {
        private MRAAInterop.Result reason;

        public MRAAException(MRAAInterop.Result reason)
        {
            this.reason = reason;
        }

        public override string Message
        {
            get
            {
                return "MRAA Exception:" + this.reason.ToString();
            }
        }
    }
}
