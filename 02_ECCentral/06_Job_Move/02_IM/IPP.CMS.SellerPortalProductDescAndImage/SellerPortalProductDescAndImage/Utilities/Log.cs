using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.Utilities
{
    public class Log
    {
        public static void WriteLog(string content, string fileRelativePath)
        {
            WriteLog(content, fileRelativePath, true);
        }

        public static void WriteLog(string content, string fileRelativePath, bool tagDateTime)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileRelativePath);
            StreamWriter sw = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                }
                sw = new StreamWriter(filePath, true);
                FileInfo fi = new FileInfo(filePath);
                string fileName;
                if (fi.Length >= 1024 * 1024 * 5)
                {
                    fileName = fi.FullName.Substring(0, fi.FullName.Length - 4) + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(filePath);
                    sw.Close();
                    if (!File.Exists(fileName))
                    {
                        File.Move(filePath, fileName);
                        sw = File.CreateText(filePath);
                    }
                    else
                        sw = File.AppendText(filePath);
                }

                sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
                sw.WriteLine(content);
            }
            catch (Exception ex)
            {
                if (sw != null)
                {
                    sw.WriteLine(ex.ToString());
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }
}
