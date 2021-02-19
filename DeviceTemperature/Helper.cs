using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTemperature {
    public sealed class Helper {


        public static int ConvertStringHexToInt(string hex0x0) {
            try {
                int value = (int)new System.ComponentModel.Int32Converter().ConvertFromString(hex0x0);
                return value;
            } catch (Exception ex) {
                throw new Exception($"Error converting hex value {hex0x0} to integer.", ex);
            }
        }

        public static SmartAttributeCollection GetSmartRegisters(string textRegisters) {
            var collection = new SmartAttributeCollection();

            try {
                var splitOnCRLF = Resource.SmartAttributes.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in splitOnCRLF) {
                    var splitLineOnComma = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string register = splitLineOnComma[0].Trim();
                    string attributeName = splitLineOnComma[1].Trim();

                    collection.Add(new SmartAttribute(Helper.ConvertStringHexToInt(register), attributeName));
                }
            } catch (Exception ex) {
                throw new Exception("GetSmartRegisters failed with error " + ex);
            }

            return collection;
        }

        public sealed class SmartAttribute {
            public SmartAttribute(int register, string attributeName) {
                this.Register = register;
                this.Name = attributeName;
            }

            public int Register { get; set; }
            public string Name { get; set; }

            public int Current { get; set; }
            public int Worst { get; set; }
            public int Threshold { get; set; }
            public int Data { get; set; }
            public bool IsOK { get; set; }

            public bool HasData
            {
                get
                {
                    if (Current == 0 && Worst == 0 && Threshold == 0 && Data == 0)
                        return false;
                    return true;
                }
            }
        }

        public class SmartAttributeCollection : List<SmartAttribute> {

            public SmartAttributeCollection() {

            }

            public SmartAttribute GetAttribute(int registerID) {
                foreach (var item in this) {
                    if (item.Register == registerID)
                        return item;
                }

                return null;
            }
        }

       

    }
}
