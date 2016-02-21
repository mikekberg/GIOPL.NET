using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.SparkFunBoards.NDOF13033
{
    public class NDOF13033DeviceException : Exception
    {
        public NDOF13033DeviceException(string message) : base(message) { }
    }
}
