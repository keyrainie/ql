using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GiveERPCustomerPoint
{
    public class TxtFileLoger : ILog
    {
        #region Member Variables

        private string logFilePath;
        private string logFileName;

        #endregion

        #region Constructors

        public TxtFileLoger(string logFileName)
        {
            string currentDir = System.Threading.Thread.GetDomain().BaseDirectory;
            this.logFileName = logFileName.Replace("/", "\\").Trim('\\');
            this.logFilePath = Path.Combine(currentDir, string.Format("log\\{0}", logFileName));
        }

        #endregion

        #region ILog Members

        /// <summary>
        /// Write Log with string context
        /// </summary>
        /// <param name="context"></param>
        public void WriteLog(string context)
        {
            if (!string.IsNullOrEmpty(logFilePath))
            {
                FileStream fileStream = null;
                if (File.Exists(logFilePath))
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    Directory.CreateDirectory(logFilePath.Substring(0, logFilePath.LastIndexOf("\\")));
                    fileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write);
                }

                StreamWriter writer = new StreamWriter(fileStream);
                try
                {
                    FileInfo fi = new FileInfo(logFilePath);
                    string fileName;
                    if (fi.Length >= 1024 * 1024 * 5)
                    {
                        fileName = fi.FullName.Substring(0, fi.FullName.Length - 4) + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(logFilePath);
                        writer.Close();
                        if (!File.Exists(fileName))
                        {
                            File.Move(logFilePath, fileName);
                            writer = File.CreateText(logFilePath);
                        }
                        else
                            writer = File.AppendText(logFilePath);
                    }
                    writer.WriteLine(string.Empty.PadLeft(20, '=') + DateTime.Now.ToString() + string.Empty.PadLeft(20, '='));
                    writer.WriteLine(context);
                    writer.Flush();
                    writer.Close();
                }
                finally
                {
                    fileStream.Close();
                    writer.Close();
                }

            }
        }


        /// <summary>
        /// Write log with a exception
        /// </summary>
        /// <param name="exception"></param>
        public void WriteLog(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Exception:  {0}{1}", DateTime.Now.ToString(), Environment.NewLine);
            sb.AppendFormat("Message:{0}", exception.Message);
            sb.AppendLine();
            sb.AppendFormat("Source:{0}", exception.Source);
            sb.AppendLine();
            sb.AppendFormat("TargetSite:{0}", exception.TargetSite);
            sb.AppendLine();
            sb.AppendFormat("StackTrace:{0}", exception.StackTrace);
            sb.AppendLine();
            sb.AppendLine();

            string exceptionContent = sb.ToString();

            WriteLog(exceptionContent);
        }

        #endregion
    }

}