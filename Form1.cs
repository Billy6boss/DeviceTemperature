using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceTemperature {
    public partial class Temperature : Form {
        private List<deviceModle> deviceList = new List<deviceModle>();
        private ScanDevice scan = new ScanDevice();

        public Temperature() {
            InitializeComponent();
            smartUpdatetick.Enabled = true;
        }

        private void Temperature_Load(object sender, EventArgs e) {
            int maxLength = this.comboBox1.Width;

            deviceList = scan.getDevices();

            foreach (deviceModle thisDevice in deviceList) {
                comboBox1.Items.Add(thisDevice.modleName);
                if (TextRenderer.MeasureText(thisDevice.modleName,comboBox1.Font).Width > maxLength)
                {
                    maxLength = TextRenderer.MeasureText(thisDevice.modleName, comboBox1.Font).Width + 15;
                }
            }

            comboBox1.Width = maxLength;
            comboBox1.Padding = new Padding(1, 1, 10, 1);
            ScanDevice.getSmartTable(deviceList);

            comboBox1.SelectedIndex = 0;

            updateTemperture();

            this.Width = maxLength + AirflowTemp.Width + temperatureText.Width + 35 ;
        }

        private void updateTemperture() {

            ScanDevice.getSmartTable(deviceList);
            AirflowTemp.Text = "0℃";

            foreach (deviceModle thisDeivice in deviceList) {

                if (!thisDeivice.busType.Equals("PCIe"))
                {
                    thisDeivice.deviceTemperature = thisDeivice.SmartAttributes[27].Current + "℃";
                    
                }

                if ((comboBox1.SelectedItem).Equals(thisDeivice.modleName)) {
                    temperatureText.Text = thisDeivice.deviceTemperature;
                    if (!thisDeivice.busType.Equals("PCIe"))
                        AirflowTemp.Text = thisDeivice.SmartAttributes[23].Current + "℃";
                }

            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            updateTemperture();
        }

        private void timer1_Tick(object sender, EventArgs e) {

            updateTemperture();
           
        }
    }
}
