using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using System.Net;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.IO;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Common
{
    internal static class Util
    {
        private static object LockObject = new object();

        internal static MessageHeader CreateMessageHeader()
        {
            return new MessageHeader
            {
                CompanyCode = Config.CompanyCode,
                FromSystem = Dns.GetHostName(),
                Language = "zh-CN",
                OperationUser = new OperationUser(Config.UserDisplayName, Config.UserLoginName, Config.StoreSourceDirectoryKey, Config.CompanyCode)
            };
        }

        internal static string GetHostIP()
        {
            IPAddress[] address = Dns.GetHostAddresses(Dns.GetHostName());
            string ip = string.Empty;
            for (int i = 0; i < address.Length; i++)
            {
                if (!address[i].IsIPv6SiteLocal)
                {
                    ip = address[i].ToString();
                    break;
                }
            }

            if (address.Length > 0
                && string.IsNullOrEmpty(ip))
            {
                ip = address[0].ToString();
            }

            return ip;
        }

        internal static void WriteLog(string log, JobContext context, bool isOutputToScreen)
        {
            //lock (LockObject)
            //{
            if (context != null)
            {
                context.Message += string.Format("{0}\r\n", log);
            }

            if (isOutputToScreen)
            {
                Console.WriteLine(log);
            }
            //}
        }

        internal static void WriteLog(string log, JobContext context)
        {
            WriteLog(log, context, true);
        }

        internal static void WriteLog(string log)
        {
            WriteLog(log, null);
        }

        internal static void WriteFile(string path, string content)
        {
            WriteFile(path, content, false);
        }

        internal static void WriteFile(string path, string content, bool isCreate)
        {
            lock (LockObject)
            {
                FileStream fs = null;
                FileInfo fi = new FileInfo(path);

                if (isCreate || !File.Exists(path))
                {
                    string dirPath = fi.DirectoryName;
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    //File.Create(path);
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
                }
                else
                {
                    long size = fi.Length;

                    if (size / 1024 / 1024 > 3) //如果该日志文件已大于3M，则移动到新文件
                    {
                        string name = fi.Name;

                        string newName = string.Format("{0}-{1}", DateTime.Now.ToString("yyyyMMdd"), name);

                        string newPath = path.Replace(name, newName);
                        DateTime fileCreateDate = fi.CreationTime;
                        if (File.Exists(newPath))
                        {
                            int i = 1;
                            do
                            {
                                newName = string.Format("{0}-{1}-{2}", fileCreateDate.ToString("yyyyMMdd"), i, name);
                                newPath = path.Replace(name, newName);
                                i++;
                            } while (File.Exists(newPath));
                        }

                        fi.MoveTo(newPath);

                        fi.Refresh();

                        WriteFile(path, content, true);
                        return;
                    }
                    else
                    {
                        fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
                    }
                }

                using (fs)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(content);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }

    }

}
