using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.Config
{
    public class PinBoardConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Pins", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(PinConfigSection), AddItemName = "Pin")]
        public PinsConfigCollection Pins
        {
            get
            {
                return (PinsConfigCollection)this["Pins"];
            }
        }

        [ConfigurationProperty("Name", IsDefaultCollection = false)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
        }
    }
}
