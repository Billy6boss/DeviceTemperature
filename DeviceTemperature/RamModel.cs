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


        public enum MemeryType {
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

        public string getCapcityUnit(long capacity) {

            return  Math.Round((capacity / Math.Pow(1024,3)),3 )+ "GB";
        }

        public void getTypeBySpeed(RamModel thisRam) {

            int clockSpeed;

            if(thisRam.speed.Length > 5) {
                clockSpeed = Convert.ToInt32(thisRam.speed.Remove(4));
            } else {
                clockSpeed = 0;   
            }

            if (clockSpeed <= 1066)
                thisRam.type = 21;
            else if (clockSpeed > 2133)
                thisRam.type = 26;
            else
                thisRam.type = 24;
        }


    }
}
