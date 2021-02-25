using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;

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
                if (TextRenderer.MeasureText(thisDevice.modleName, comboBox1.Font).Width > maxLength) {
                    maxLength = TextRenderer.MeasureText(thisDevice.modleName, comboBox1.Font).Width + 15;
                }
            }

            comboBox1.Width = maxLength;
            comboBox1.Padding = new Padding(1, 1, 10, 1);
            ScanDevice.getSmartTable(deviceList);

            comboBox1.SelectedIndex = 0;

            updateTemperture();

            this.Width = maxLength + AirflowTemp.Width + temperatureText.Width + 35;
        }

        private void updateTemperture() {

            ScanDevice.getSmartTable(deviceList);
            AirflowTemp.Text = "0℃";

            foreach (deviceModle thisDeivice in deviceList) {

                if (!thisDeivice.busType.Equals("PCIe")) {
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

        private void getPCbtn_Click(object sender, EventArgs e) {


            try {
            PCinfoModel thisPC = scan.getPcInfo();

                string fileName = Interaction.InputBox("請輸入你的中文姓名與英文名子", "輸入名稱", "帥哥-FBI");
            fileName += "-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";


                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\" + fileName)) {

                    file.WriteLine("CPU： " + thisPC.proesser);
                    file.WriteLine("MotherBoard： " + thisPC.motherBoard);

                    int i = 0;
                    foreach (RamModel thisRam in thisPC.rams) {
                        i++;
                        if(thisRam.type == 0) {
                            thisRam.getTypeBySpeed(thisRam);
                        }

                        file.WriteLine("RAM" + i + "： " + thisRam.Maker + " " +
                            Enum.GetName(typeof(RamModel.MemeryType), thisRam.type) + " " +
                            thisRam.speed + " " +
                            thisRam.getCapcityUnit(thisRam.capacity));
                    }
                    i = 0;
                    foreach (deviceModle thisDrive in thisPC.drives) {
                        i++;
                        file.WriteLine("Drive" + i + "： " + thisDrive.modleName + " " + thisDrive.Size);
                    }


                    file.WriteLine("Graphic Card： " + thisPC.videoCard);
                    file.WriteLine("OS： " + thisPC.os);
                    file.WriteLine("----------------------------------------------------------------");


                }

            } catch (Exception ex) {

                System.Diagnostics.Trace.Assert(false, ex.ToString());
                string debugTxetPath =  "C:\\getPCbtn_ClickDebug.txt";
                using (TextWriterTraceListener _DebugLog = new TextWriterTraceListener(debugTxetPath)) {
                    Trace.Listeners.Add(_DebugLog);
                    Trace.AutoFlush = true;
                    Trace.WriteLine("Button Area error:" + ex.StackTrace.ToString());
                    Trace.Listeners.Clear();
                    Trace.Close();
                }
            }

            MessageBox.Show("Thank you! I got you!", "U R the best");
            Process.Start(@"C:\");
        }



    }
}
