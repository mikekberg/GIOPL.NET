using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL
{
    public enum I2CModes : int
    {
        /// <summary>
        /// Standard sets speed up to 100 kHz
        /// </summary>
        Standard = 0,
        /// <summary>
        /// Fast sets speed up t0 400 kHz
        /// </summary>
        Fast = 1,
        /// <summary>
        /// High sets speed up to 3.4 MHz
        /// </summary>
        High = 2
    }
}
