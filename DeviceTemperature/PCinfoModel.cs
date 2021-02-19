using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTemperature {
    class PCinfoModel {

        public String proesser { get; set; }

        public string motherBoard { get; set; }

        public List<RamModel> rams { get; set; } = new List<RamModel>();
        public string videoCard { get; set; }

        public List<deviceModle> drives { get; set; } = new List<deviceModle>();

        public string os { get; set; }
    }
}
