using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GIOPL.Config
{
    public class PinConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Number", IsDefaultCollection = false)]
        public int Number
        {
            get
            {
                return (int)this["Number"];
            }
        }

        [ConfigurationProperty("Mode", IsDefaultCollection = false)]
        public string Mode
        {
            get
            {
                return (string)this["Mode"];
            }
        }

        [ConfigurationProperty("DirectionPin", IsDefaultCollection = false)]
        public int DirectionPin
        {
            get
            {
                return (int)this["DirectionPin"];
            }
        }

        [ConfigurationProperty("PullupPin", IsDefaultCollection = false)]
        public int PullupPin
        {
            get
            {
                return (int)this["PullupPin"];
            }
        }
    }
}
