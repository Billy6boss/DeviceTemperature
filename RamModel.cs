using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTemperature {
    class RamModel {

        public String Maker { get; set; }

        public String speed { get; set; }

        public long capacity { get; set; }

        public int type { get; set; }


        enum MemeryType {
            Unknown,
            Other,
            DRAM,
            SynchronousDRAM,
            CacheDRAM,
            EDO,
            EDRAM,
            VRAM,
            SRAM,
            RAM,
            ROM,
            Flash,
            EEPROM,
            FEPROM,
            EPROM,
            CDRAM,
            ThreeDRAM,
            SDRAM,
            SGRAM,
            RDRAM,
            DDR,
            DDR2,
            DDR2FBDIMM,
            DDR3=24,
            FBD2,
            DDR4
        }

        public string getCapcityUnit(int capacity) {

            return (capacity / 1024 ^ 3) + "GB";
        }
    }
}
