using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Net;
using System.Data.SqlClient;
using System.Data;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace SendNoticeMailForCustomerPoint
{
    class Program : IJobAction
    {
        private ILog log = LogerManger.GetLoger();
        private JobContext context = null;
        private string path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
  
        private  void DoJob()
        {
            log.WriteLog("Job Start");
            PrintMessage("Job Start.");

            string MailSubject = GetMailTemplate("MailSubjectTemplate.txt");
            string MailBody = GetMailTemplate("MailBodyTemplate.txt");
            int result = 0;
            try
            {
                DataCommand dc = DataCommandManager.GetDataCommand("GetInsertAsyncEmail");
                dc.SetParameterValue("@mailSubject", MailSubject);
                dc.SetParameterValue("@mailBody", MailBody);

                result = dc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.WriteLog(e);
                PrintMessage(e.Message);
                throw e;
            }
            log.WriteLog(result + " e-mail has been sent,Job End");
            PrintMessage(result + " e-mail has been sent,Job End");
        }        

        private string GetMailTemplate(string txtPath)
        {
            string Template = "";
            FileStream fs = new FileStream(Path.Combine(path,txtPath), FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            try
            {
                Template = sr.ReadToEnd();
            }
            catch (IOException ex)
            {
                log.WriteLog(ex);
                throw ex;
            }
            finally
            {
                sr.Close();
                fs.Close();
            }
            return Template;
        }

        static void Main(string[] args)
        {
            Program biz = new Program();
            if (string.Compare(DA.GetSys_Configuration_NewPointSwitch(), "Y") == 0)
            {
                Biz newBiz = new Biz();
                newBiz.SendExpireEmailEmail(biz.context);
            }
            else
            {                
                biz.DoJob();
            }
        }

        public void Run(JobContext context)
        {
            this.context = context;
            PrintMessage("Job运行开始！");
            if (string.Compare(DA.GetSys_Configuration_NewPointSwitch(), "Y") == 0)
            {
                Biz newBiz = new Biz();
                newBiz.SendExpireEmailEmail(context);
            }
            else
            {
                DoJob();
            }
            PrintMessage("Job运行完成！");
        }

        private void PrintMessage(string msg)
        {
            if (this.context != null)
            {
                context.Message += msg + Environment.NewLine;
            }
            else
            {
                Console.WriteLine(msg);
                if (msg == "Stop")
                {
                    Console.ReadLine();
                }
            }
        }
    }
}