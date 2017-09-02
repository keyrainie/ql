using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using IPPOversea.Invoicemgmt.SyncDataToFinancePay.DAL;
using IPPOversea.Invoicemgmt.SyncDataToFinancePay.Biz;
using System.Configuration;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.Data;

using Newegg.Oversea.Framework.ServiceConsole.Client;
using System.ServiceModel;
using Newegg.Oversea.Framework.Core.WebReference;
using Newegg.Oversea.Framework.Contract;

namespace IPPOversea.Invoicemgmt.SyncDataToFinancePay.Biz
{
    public static class SyncDataBL
    {
        #region Field

        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);
        
        #endregion

        public static void DoWork(AutoResetEvent are)
        {
            OnShowInfo("开始同步数据");
            try
            {
                //添加业务逻辑
                List<string> processNameList = Settings.SyncDataProcessList;
                int syncDataCount = Settings.SyncDataCount;

                for (int i = 0; i < processNameList.Count; i++)
                {
                    SyncDataDAL.SyncDataToFinancePayProcess(syncDataCount, processNameList[i]);
                }                    
                are.Set();
            }
            catch (Exception ex)
            {
                are.Set();
                throw ex;
            }
        }
        /// <summary>
        /// 发送邮件信息
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        public static void SendMail(string subject, string mailBody)
        {
            MailDAL.SendEmail(Settings.EmailAddress, subject, mailBody);
        }
        
        public static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo!=null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }
    }
}
