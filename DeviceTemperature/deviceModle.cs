using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeviceTemperature.Helper;

namespace DeviceTemperature {
    public class deviceModle {

        public deviceModle() {
            SmartAttributes = new SmartAttributeCollection();
        }

        public int PhysicalDriveNumber { get; set; }
        public string modleName { get; set; }
        public string deviceTemperature { get; set; }
        public SmartAttributeCollection SmartAttributes { get; set; }
        public String busType { get; set; }

        public String Size { get; set; }
        public deviceModle(int number,string name , string temper,string busType) {
            this.PhysicalDriveNumber = number;
            this.modleName = name;
            this.deviceTemperature = temper;
            this.busType = busType;

        }
    }

}
