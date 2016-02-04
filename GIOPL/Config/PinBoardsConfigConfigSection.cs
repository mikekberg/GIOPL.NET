using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GIOPL.Config
{
    public class PinBoardsConfigConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("PinBoards", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(PinBoardConfigSection), AddItemName = "Board")]
        public GenericConfigurationElementCollection<PinBoardConfigSection> PinBoards
        {
            get
            {
                return (GenericConfigurationElementCollection<PinBoardConfigSection>) this["PinBoards"];
            }
        }
    }
}
