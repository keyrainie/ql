using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SolrImportDataAPPJob
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        // Fields
        private static string httpRequestType   = ConfigUtility.GetConfigValue("HttpRequestType");
        private static string solrImportDataURL = ConfigUtility.GetConfigValue("ImportDataSolrURL");
        private static int timeOut              = Convert.ToInt32(ConfigUtility.GetConfigValue("TimeOut"));

        // Methods
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            HttpWebRequest request   = null;
            HttpWebResponse response = null;
            StreamReader txtReader   = null;

            Console.WriteLine("***************************************");
            Console.WriteLine("Solr FullImportData Begin!");
            try
            {
                request              = (HttpWebRequest)WebRequest.Create(solrImportDataURL);
                request.Method       = httpRequestType;
                request.ContentType  = "UTF-8";
                request.Timeout      = timeOut;
                response             = (HttpWebResponse)request.GetResponse();
                txtReader            = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

                XmlDocument document = new XmlDocument();
                document.Load(txtReader);

                WriteLog(document.InnerXml);

                Console.WriteLine("QTime:" + document.SelectNodes("response/lst/int").Item(1).InnerXml);
                Console.WriteLine("Solr FullImportData Succeed!");
            }
            catch (Exception exception)
            {
                WriteLog(exception.Message + exception.StackTrace);
                Console.WriteLine("Solr FullImportData Failed!");
            }
            finally
            {
                if (txtReader != null) txtReader.Close();
                if (response != null) response.Close();
            }

            Console.WriteLine("***************************************");
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="text">The text.</param>
        private static void WriteLog(string text)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs";
            if (!System.IO.Directory.Exists(logPath))
            {
                System.IO.Directory.CreateDirectory(logPath);
            }

            string logFile = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
            logFile        = Path.Combine(logPath, logFile);

            StreamWriter writer = new StreamWriter(logFile, true, Encoding.UTF8);
            try
            {
                writer.Write(string.Format("********************{0}*******************", DateTime.Now));
                writer.Write(Environment.NewLine);
                writer.Write(text);
                writer.Write(Environment.NewLine);
            }
            catch
            {
            }
            finally
            {
                writer.Close();
            }
        }
    }
}
