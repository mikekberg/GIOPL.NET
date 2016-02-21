using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.SparkFunBoards.NDOF13033
{
    public enum GyroODR : byte
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

}
