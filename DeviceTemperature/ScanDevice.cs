using DeviceTemperature.Win32Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceTemperature {
    class ScanDevice {


        public List<deviceModle> getDevices() {

            List<deviceModle> devices = new List<deviceModle>();
            foreach (var device in new ManagementObjectSearcher(@"SELECT * FROM Win32_DiskDrive").Get()) {

                deviceModle thisdevices = new deviceModle();


                thisdevices.modleName = device["Model"]?.ToString().Trim();

                string diskname = device["DeviceID"].ToString();
                thisdevices.PhysicalDriveNumber = int.Parse(diskname.Substring(diskname.Length - 1,1));



                System.IntPtr hDevice = Kernel.CreateFile(diskname,
                                            Kernel.GENERIC_READ | Kernel.GENERIC_WRITE,
                                            Kernel.FILE_SHARE_READ | Kernel.FILE_SHARE_WRITE,
                                            0,
                                            Kernel.OPEN_EXISTING,
                                            0,
                                            0);

                STORAGE_PROPERTY_QUERY Query = new STORAGE_PROPERTY_QUERY();
                STORAGE_DEVICE_DESCRIPTOR DevDesc = new STORAGE_DEVICE_DESCRIPTOR();
                string bustype = "";
                uint bytesReturned = 0;
                Query.PropertyId = 0;
                Query.QueryType = 0;

                if (0 != Kernel.DeviceIoControl(hDevice,                                // device handle
                                                Kernel.IOCTL_STORAGE_QUERY_PROPERTY,            // info of device property
                                                ref Query,
                                                (uint)Marshal.SizeOf(Query),            // input data buffer
                                                ref DevDesc,
                                                (uint)Marshal.SizeOf(DevDesc),              // output data buffer
                                                ref bytesReturned,                          // out's length
                                                IntPtr.Zero)) {
                    switch (DevDesc.BusType) {
                        case STORAGE_BUS_TYPE.BusTypeUnknown:
                            bustype = Kernel.DISK_TYPE_UNKNOWN_TEXT;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeScsi:
                            bustype = Kernel.DISKTYPE_SCSI;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeAtapi:
                            bustype = Kernel.DISKTYPE_ATAPI;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeAta:
                            bustype = Kernel.DISKTYPE_ATA;
                            break;
                        case STORAGE_BUS_TYPE.BusType1394:
                            bustype = Kernel.DISKTYPE_1394;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeSsa:
                            bustype = Kernel.DISKTYPE_SSA;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeFibre:
                            bustype = Kernel.DISKTYPE_FIBRE;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeUsb:
                            bustype = Kernel.DISKTYPE_USB;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeRAID:
                            bustype = Kernel.DISKTYPE_RAID;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeSata:
                            bustype = Kernel.DISKTYPE_SATA;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeMax:
                            bustype = Kernel.DISKTYPE_MAX;
                            break;
                        case STORAGE_BUS_TYPE.BusTypeNvme:
                            bustype = Kernel.DISKTYPE_MAX;
                            break;
                        default:
                            break;
                    }
                }

                thisdevices.busType = bustype;
                devices.Add(thisdevices);
                Kernel.CloseHandle(hDevice);

            }
            return devices;
        }



        //-----------SMART------
        public static bool getSmartTable(List<deviceModle> devices) {
            System.IntPtr hDevice;
            string diskname;
            uint bytesReturned = 0;
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParamsSmart outParam = new SendCmdOutParamsSmart();

            foreach (deviceModle thisDevice in devices) {
                try {

                    diskname = "\\\\.\\PhysicalDrive" + thisDevice.PhysicalDriveNumber.ToString();

                    hDevice = Kernel.CreateFile(diskname,
                                                Kernel.GENERIC_READ | Kernel.GENERIC_WRITE,
                                                Kernel.FILE_SHARE_READ | Kernel.FILE_SHARE_WRITE,
                                                0,
                                                Kernel.OPEN_EXISTING,
                                                0,
                                                0);
                    if (hDevice.ToInt32() <= 0)
                        continue;


                    if (thisDevice.busType.Equals("PCIe"))
                    {
                        TStorageQueryWithBuffer nptwb = new TStorageQueryWithBuffer();
                        TStorageProtocolSpecificData SDATASize = new TStorageProtocolSpecificData();

                        nptwb.ProtocolSpecific.ProtocolType = TStroageProtocolType.ProtocolTypeNvme;
                        nptwb.ProtocolSpecific.DataType = (uint)TStorageProtocolNVMeDataType.NVMeDataTypeLogPage;
                        nptwb.ProtocolSpecific.ProtocolDataRequestValue = 0x02;
                        nptwb.ProtocolSpecific.ProtocolDataRequestSubValue = 0xFFFFFFFF;
                        nptwb.ProtocolSpecific.ProtocolDataOffset = (uint)Marshal.SizeOf(SDATASize);
                        nptwb.ProtocolSpecific.ProtocolDataLength = 512;
                        nptwb.Query.PropertyId = TStoragePropertyId.StorageAdapterProtocolSpecificProperty;
                        nptwb.Query.QueryType = TStorageQueryType.PropertyStandardQuery;
                        UInt32 dwReturned = 0;

                        if (0 != Kernel.DeviceIoControl(
                            hDevice,
                            Kernel.IOCTL_STORAGE_QUERY_PROPERTY,
                            ref nptwb,
                            (uint)Marshal.SizeOf(nptwb),
                            ref nptwb,
                            (uint)Marshal.SizeOf(nptwb),
                            ref dwReturned,                         // out's length
                            IntPtr.Zero))
                        {

                            int StartIndex = 1, EndIndex = 2;
                            ulong RawData = 0;


                            while (EndIndex >= StartIndex)
                            {
                                RawData *= 256;
                                RawData += nptwb.Buffer[EndIndex];
                                EndIndex--;
                            }

                            thisDevice.deviceTemperature = ((long)RawData - 273) + "℃";

                        }
                    }
                    else
                    {
                        thisDevice.SmartAttributes = new Helper.SmartAttributeCollection();
                        thisDevice.SmartAttributes.AddRange(Helper.GetSmartRegisters(Resource.SmartAttributes));



                        inParam.cBufferSize = 512;
                        inParam.bDriveNumber = (byte)thisDevice.PhysicalDriveNumber;
                        inParam.irDriveRegs.bFeaturesReg = 0xD0;
                        inParam.irDriveRegs.bSectorCountReg = 1;
                        inParam.irDriveRegs.bSectorNumberReg = 1;
                        inParam.irDriveRegs.bCylLowReg = 0x4F;
                        inParam.irDriveRegs.bCylHighReg = 0xC2;
                        inParam.irDriveRegs.bDriveHeadReg = 0xA0;
                        inParam.irDriveRegs.bCommandReg = 0xB0;

                        if (0 != Kernel.DeviceIoControl(hDevice,
                                                Kernel.SMART_RCV_DRIVE_DATA,
                                                ref inParam,
                                                (uint)Marshal.SizeOf(inParam),
                                                ref outParam,
                                                (uint)Marshal.SizeOf(outParam),
                                                ref bytesReturned,
                                                IntPtr.Zero))
                        {

                            for (int i = 0; i < 42; ++i)
                            {

                                Byte[] bytes = outParam.bBuffer;

                                int id = bytes[i * 12 + 2];

                                int flags = bytes[i * 12 + 4]; // least significant status byte, +3 most significant byte, but not used so ignored.
                                                               //bool advisory = (flags & 0x1) == 0x0;
                                bool failureImminent = (flags & 0x1) == 0x1;
                                //bool onlineDataCollection = (flags & 0x2) == 0x2;

                                int value = bytes[i * 12 + 5];
                                int worst = bytes[i * 12 + 6];
                                int vendordata = BitConverter.ToInt32(bytes, i * 12 + 7);
                                //if (id == 0) continue;


                                foreach (Helper.SmartAttribute attr in thisDevice.SmartAttributes)
                                {

                                    if (attr.Register == id)
                                    {
                                        attr.Current = value;
                                        attr.Worst = worst;
                                        attr.Data = vendordata;
                                        attr.IsOK = failureImminent == false;
                                    }
                                }

                            }
                        }
                    }
                    Kernel.CloseHandle(hDevice);
                } catch (Exception ex) {

                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            }
            return true;
        }


        public PCinfoModel getPcInfo() {
            PCinfoModel thisPC = new PCinfoModel();

            try { 
                foreach (var CPU in new ManagementObjectSearcher(@"SELECT * FROM Win32_Processor").Get()) { 

                    thisPC.proesser = CPU["Name"].ToString();
                }

                foreach (var MTB in new ManagementObjectSearcher(@"SELECT * FROM Win32_BaseBoard").Get()) {
                    thisPC.motherBoard = MTB["Manufacturer"].ToString() + " " + MTB["Product"].ToString();
                }

                foreach (var RAM in new ManagementObjectSearcher(@"Select * From Win32_PhysicalMemory").Get()) {

                    RamModel thisRam = new RamModel();
                    thisRam.Maker =RAM["Manufacturer"].ToString();
                    thisRam.speed = RAM["Speed"].ToString() + "MHz";
                    thisRam.type = Convert.ToInt32(RAM["MemoryType"]);
                    thisRam.capacity = Convert.ToInt64(RAM["Capacity"]);

                    thisPC.rams.Add(thisRam);
                }

                foreach (var Drive in new ManagementObjectSearcher(@"Select * From Win32_DiskDrive").Get()) {

                    deviceModle thisdevice = new deviceModle();

                    thisdevice.modleName = Drive["Model"].ToString();
                    thisdevice.Size =Math.Round((Convert.ToInt64(Drive["Size"])/ Math.Pow(1024,3)),3) + "GB";

                    thisPC.drives.Add(thisdevice);
                
                }

                foreach (var GPU in new ManagementObjectSearcher(@"Select * From Win32_VideoController").Get()) {

                    thisPC.videoCard = GPU["Name"].ToString();
                }

                foreach (var OS in new ManagementObjectSearcher(@"Select * From Win32_OperatingSystem").Get()) {

                    thisPC.os = OS["Name"].ToString().Split('|')[0] +" "+OS["OSArchitecture"].ToString() + " ";

                    string version = "";

                    #region Switch OS Name case
                    switch (OS["BuildNumber"].ToString()) {
                        case "2600":
                            version = "Windows XP";
                            break;
                        case "2600.1105-1106":
                            version = "Windows XP, Service Pack 1";
                            break;
                        case "2600.218":
                            version = "Windows XP, Service Pack 2";
                            break;
                        case "3790.118":
                            version = "Windows Server 2003, Service Pack 1";
                            break;
                        case "3790":
                            version = "Windows Server 2003 R2";
                            break;
                        case "6000":
                            version = "Windows Vista";
                            break;
                        case "4500":
                            version = "Windows Home Server";
                            break;
                        case "6001":
                            version = "Windows Vista, Service Pack 1";
                            break;
                        case "6002":
                            version = "Windows Vista, Service Pack 2";
                            break;
                        case "7600":
                            version = "Windows 7";
                            break;
                        case "7601":
                            version = "Windows Server 2008 R2, Service Pack 1";
                            break;
                        case "8400":
                            version = "Windows Home Server 2011";
                            break;
                        case "9200":
                            version = "Windows 8";
                            break;
                        case "9600":
                            version = "Windows 8.1";
                            break;
                        case "10240":
                            version = "Windows 10, Version 1507";
                            break;
                        case "10586":
                            version = "Windows 10, Version 1511";
                            break;
                        case "14393":
                            version = "Windows 10, Version 1607";
                            break;
                        case "15063":
                            version = "Windows 10, Version 1703";
                            break;
                        case "16299":
                            version = "Windows 10, Version 1709";
                            break;
                        case "17134":
                            version = "Windows 10, Version 1803";
                            break;
                        case "17763":
                            version = "Windows 10, Version 1809";
                            break;
                        case "6003":
                            version = "Windows Server 2008, Service Pack 2, Rollup KB4489887";
                            break;
                        case "18362":
                            version = "Windows 10, Version 1903";
                            break;
                        case "18363":
                            version = "Windows 10, Version 1909";
                            break;
                        case "19041":
                            version = "Windows 10, Version 2004";
                            break;
                        case "19042":
                            version = "Windows 10, Version 20H2";
                            break;
                        default:
                            version = "Can't Find " + OS["BuildNumber"];
                            break;
                    }
                    #endregion

                    thisPC.os += version;

                }


            } catch (Exception ex) {
                System.Diagnostics.Trace.Write(ex.ToString());
                return null;
            }

            return thisPC;


        }


    }
}