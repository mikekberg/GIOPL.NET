using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.Config
{
    public class PinsConfigCollection : GenericConfigurationElementCollection<PinConfigSection>
    {
        [ConfigurationProperty("PreConfigPin", IsDefaultCollection = false)]
        public int PreConfigPin
        {
            get
            {
                return (int)this["PreConfigPin"];
            }
        }

        [ConfigurationProperty("PreConfigValue", IsDefaultCollection = false)]
        public string PreConfigValue
        {
            get
            {
                return (string)this["PreConfigValue"];
            }
        }

        [ConfigurationProperty("PostConfigValue", IsDefaultCollection = false)]
        public string PostConfigValue
        {
            get
            {
                return (string)this["PostConfigValue"];
            }
        }
    }
}
