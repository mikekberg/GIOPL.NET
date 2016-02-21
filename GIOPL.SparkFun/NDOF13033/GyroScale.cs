using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.SparkFunBoards.NDOF13033
{
    public enum GyroScale : byte
    {
        G_SCALE_245DPS,     // 00:  245 degrees per second
        G_SCALE_500DPS,     // 01:  500 dps
        G_SCALE_2000DPS    // 10:  2000 dps
    };
}
