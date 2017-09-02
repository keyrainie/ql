using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentMgmt.JobProductRequest.Resources;
using System.Data;
using IPP.Oversea.CN.ContentMgmt.JobProductRequest.DataAccess;
using IPP.Oversea.CN.ContentMgmt.JobProductRequest.Entities;
using IPP.Oversea.CN.ContentMgmt.FtpDownload.ServiceAdapter;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections;

namespace IPP.Oversea.CN.ContentMgmt.JobProductRequest.BizProcess
{
    public class JobProductRequestBiz
    {
        private Hashtable m_Type = new Hashtable();

        public JobProductRequestBiz()
        {
            m_Type.Add("N", "新品创建");
            m_Type.Add("P", "属性更新");
            m_Type.Add("I", "图片与描述更新");
        }

        public void Start()
        {
            int err = Convert.ToInt32(ConfigurationManager.AppSettings["MaxErr"]);
            int sleep = Convert.ToInt32(ConfigurationManager.AppSettings["ErrSleep"]);
            Exception ex = null;
            for (int i = 0; i < err; i++)
                try
                {
                    start();
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                    log(string.Format(@"程序发成错误:{0}，{1}秒后重试", e.Message, sleep));
                    Thread.Sleep(sleep * 1000);
                }

            throw ex;

        }

        private void start()
        {
            StringBuilder sb = new StringBuilder();
            DataSet ds = JobProductRequestDA.GetJobProductRequest(setEntity());

            if (!(ds.Tables[0].Rows.Count > 0))
                return;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                bindRow(sb, ds.Tables[0].Rows[i]);
            string content = EmailResources.EmailContent.Replace("@CONTENT", sb.ToString());

            MailEntity entity = new MailEntity()
            {
                From = ConfigurationManager.AppSettings["EmailFrom"].ToString(),
                To = ConfigurationManager.AppSettings["EmailTo"].ToString(),
                CC = ConfigurationManager.AppSettings["EmailCC"].ToString(),
                Body = content,
                Subject = EmailResources.EmailTitle
            };

            EmailComparisonCNServiceFacade.SendProductEmail(entity);
            log("邮件发送成功");
        }

        private JobProductRequestEntity setEntity()
        {
            string CompanyCode = "8601";
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CompanyCode"].ToString()))
            {
                CompanyCode = ConfigurationManager.AppSettings["CompanyCode"].ToString();
            }

            JobProductRequestEntity entity = new JobProductRequestEntity();
            entity.Status = "A";
            entity.CompanyCode = CompanyCode;
            return entity;
        }

        private void bindRow(StringBuilder sb, DataRow dr)
        {
            string row = EmailResources.EmailRowContent;
            for (int i = 0; i < dr.Table.Columns.Count; i++)
                row = row.Replace("@" + dr.Table.Columns[i].ColumnName.ToUpper(), columnValue(dr.Table.Columns[i].ColumnName, dr) + "&nbsp;");
            sb.AppendLine(row);
        }

        private string columnValue(string name, DataRow dr)
        {
            string typeName = (dr[name] + string.Empty).ToUpper();
            if ("TYPE" == name.ToUpper() && m_Type.Contains(typeName))
                return m_Type[typeName].ToString();
            else
                return dr[name] + string.Empty;
        }

        private void log(string text)
        {
            Console.WriteLine(text);
            string path = string.Format(@"{1}log\{0}.log", DateTime.Now.ToString("yyyyMMdd"), AppDomain.CurrentDomain.BaseDirectory);
            string folder = string.Format(@"{0}log",AppDomain.CurrentDomain.BaseDirectory);
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            File.AppendAllText(path, string.Format("{0} {1}", DateTime.Now.ToString("hh:mm:ss"), text + '\r' + '\n'));
        }

    }
}
