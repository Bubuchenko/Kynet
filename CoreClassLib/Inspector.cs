using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;


namespace KynetLib
{
    public class Inspector
    {
        public static string GetUsername()
        {
            return Environment.UserName;
        }
        public static string GetMachineName()
        {
            return Environment.MachineName;
        }
        public static string GetDomain()
        {
            return Environment.UserDomainName;
        }
        public static string GetOperatingSystem()
        {
            return (new ComputerInfo { }).OSFullName;
        }

        public static string GetCountry()
        {
            return RegionInfo.CurrentRegion.DisplayName;
        }

        #region Extended code to determine system type
        public enum ChassisTypes
        {
            Other = 1,
            Unknown,
            Desktop,
            LowProfileDesktop,
            PizzaBox,
            MiniTower,
            Tower,
            Portable,
            Laptop,
            Notebook,
            Handheld,
            DockingStation,
            AllInOne,
            SubNotebook,
            SpaceSaving,
            LunchBox,
            MainSystemChassis,
            ExpansionChassis,
            SubChassis,
            BusExpansionChassis,
            PeripheralChassis,
            StorageChassis,
            RackMountChassis,
            SealedCasePC
        }

        public static ChassisTypes GetCurrentChassisType()
        {
            ManagementClass systemEnclosures = new ManagementClass("Win32_SystemEnclosure");

            foreach (ManagementObject obj in systemEnclosures.GetInstances())
            {

                foreach (int i in (UInt16[])(obj["ChassisTypes"]))
                {
                    if (i > 0 && i < 25)
                    {
                        return (ChassisTypes)i;
                    }
                }
            }
            return ChassisTypes.Unknown;
        }
        #endregion
        public static string GetSystemType()
        {
            return GetCurrentChassisType().ToString();
        }

        public static int GetMonitorCount()
        {
            return Screen.AllScreens.Length;
        }
        public static string GetGPU()
        {
            return new ManagementObjectSearcher("SELECT Description FROM Win32_DisplayConfiguration").Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("Description").ToString();
        }
        public static string GetCPU()
        {
            return new ManagementObjectSearcher("root\\CIMV2", "SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("Name").ToString();
        }
        public static int GetRAM()
        {
            return Convert.ToInt32((new ComputerInfo { }).TotalPhysicalMemory / 1024 / 1024);
            
        }
        public static bool GetOSType()
        {
            return Environment.Is64BitOperatingSystem;
        }

        public static string GetFingerprint()
        {
            string username = Environment.UserName.Replace(" ", "");
            string key1 = new ManagementObjectSearcher("SELECT VolumeSerialNumber FROM win32_logicaldisk").Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("VolumeSerialNumber").ToString();
            string key2 = new ManagementObjectSearcher("SELECT processorID FROM Win32_Processor").Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("processorID").ToString();
            return string.Format("{0}-{1}{2}", username, key1, key2);
        }

    }
}
