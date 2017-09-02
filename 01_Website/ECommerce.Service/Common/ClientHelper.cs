using System.Net;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace ECommerce.Facade
{
    public class ClientHelper
    {
        //获取本机的IP
        public static string getLocalIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ipEntry.AddressList[i].ToString();
                }
            }
            return string.Empty;
        }
        //获取本机的MAC
        public static string getLocalMac()
        {
            string macAddress = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["IPEnabled"].ToString() == "True")
                {
                    macAddress = mo["MacAddress"].ToString();
                }
            }
            return macAddress;
        }
    }
}
