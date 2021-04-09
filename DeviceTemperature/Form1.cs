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
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

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

        private void converBtn_Click(object sender, EventArgs e) {

            Dictionary<String, String> computerDetail;
            List<Dictionary<String, String>> computerList = new List<Dictionary<string, string>>();

            try {
                DirectoryInfo dInfo = new DirectoryInfo(@"E:\peoples");
                FileInfo[] filesInfo = dInfo.GetFiles("*.txt");

                foreach (FileInfo thisFile in filesInfo) {
                    computerDetail = new Dictionary<string, string>();
                    string[] allTheline = File.ReadAllLines(@"E:\peoples\" + thisFile.Name);
                    string chzName = thisFile.Name.Split('-')[0];
                    string engName = thisFile.Name.Split('-')[1];

                    computerDetail.Add("Name", chzName + "("+engName+")");
   
                    foreach (string thisLine in allTheline) {

                        if (thisLine.Split('：').Length >= 2) {
                            string header = thisLine.Split('：')[0];
                            string value = thisLine.Split('：')[1].Trim();

                            header = Regex.Replace(header, @"[0-9]", String.Empty).Trim();

                            if (computerDetail.ContainsKey(header)) {
                                computerDetail[header] += value + "\n";
                            } else {
                                computerDetail.Add(header, value);
                            }
                        }
                    }
                    computerList.Add(computerDetail);
                    Process.Start(@"E:\");


                }

                outExcel(computerList);
                MessageBox.Show("OK!");


            } catch (Exception ex) {

                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }


        private void outExcel(List<Dictionary<String, String>> dataList) {
            string pathFile = @"E:\員工電腦資訊2";
            string[] headers = {"Name","CPU", "MotherBoard", "RAM", "Drive", "Graphic Card", "OS" };
            Excel.Application excelApp;
            Excel._Workbook wBook;
            Excel._Worksheet wSheet;
            Excel.Range wRange;

            // 開啟一個新的應用程式
            excelApp = new Excel.Application();

            // 讓Excel文件可見
            excelApp.Visible = true;

            // 停用警告訊息
            excelApp.DisplayAlerts = false;

            // 加入新的活頁簿
            excelApp.Workbooks.Add(Type.Missing);

            // 引用第一個活頁簿
            wBook = excelApp.Workbooks[1];

            // 設定活頁簿焦點
            wBook.Activate();

            try {
                // 引用第一個工作表
                wSheet = (Excel._Worksheet)wBook.Worksheets[1];

                // 命名工作表的名稱
                wSheet.Name = "員工電腦資訊";

                // 設定工作表焦點
                wSheet.Activate();

                for (int x = 0; x < headers.Length; x++) {
                    excelApp.Cells[1, x + 1] = headers[x];
                }

                for (int i = 0; i < dataList.Count; i++) {

                    Dictionary<String, String> data = dataList[i];

                    for (int xValue = 0; xValue < headers.Length; xValue++) {

                        if (data.ContainsKey(headers[xValue])) {
                            excelApp.Cells[i + 2, xValue+1] = data[headers[xValue]];
                            excelApp.Cells[i + 2, xValue + 1].Style.WrapText = true;
                        }
                    }


                }


                // 設定總和公式 =SUM(B2:B4)
                //excelApp.Cells[5, 2].Formula = string.Format("=SUM(B{0}:B{1})", 2, 4);

                // 自動調整欄寬
                wRange = wSheet.Range[wSheet.Cells[1, 1], wSheet.Cells[dataList.Count+2, headers.Length+1]];
                wRange.Select();
                wRange.Columns.AutoFit();

                try {
                    //另存活頁簿
                    wBook.SaveAs(pathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    Console.WriteLine("儲存文件於 " + Environment.NewLine + pathFile);
                } catch (Exception ex) {
                    Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
                }
            } catch (Exception ex) {
                Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.ToString());
            }

            //關閉活頁簿
            wBook.Close(false, Type.Missing, Type.Missing);

            //關閉Excel
            excelApp.Quit();

            //釋放Excel資源
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            wBook = null;
            wSheet = null;
            wRange = null;
            excelApp = null;
            GC.Collect();

            Console.Read();
        }



    }
}
