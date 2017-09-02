using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.IO;
using IPP.OrderMgmt.SyncSoStatusToWMS.BusinessEntities;
using IPP.OrderMgmt.SyncSoStatusToWMS.Dac.Common;


namespace IPP.OrderMgmt.SyncSoStatusToWMS.Providers
{
    public class SyncSoStatusToWMS : IJobAction
    {
        public void Run(JobContext context)
        {
            if (!File.Exists("LastSysncDate.txt"))
            {
                StreamWriter sw = File.CreateText("LastSysncDate.txt");
                sw.Write("2000-01-01 00:00:00.fff");
                sw.Flush();
                sw.Close();
            }

           string str =  File.ReadAllText("LastSysncDate.txt");
           DateTime lastSyncDate;
           if (!DateTime.TryParse(str, out lastSyncDate))
           {
               lastSyncDate = DateTime.Parse("2000-01-01");
           }
            DateTime date = CommonDA.GetDbDate();
            bool isAllok = true;
           System.Collections.Generic.List<SoInfo> list = CommonDA.GetSyncSo(lastSyncDate);
           context.Message += string.Format("开始同步状态，发现{0}个订单\r\n", list.Count);
           foreach (var item in list)
           {
               try
               {
                   CommonDA.UpdateSODeclareStatus(item.SoSysno, item.SoStatus, item.StockSysNo);
               }
               catch (Exception ex)
               {
                   isAllok = false;
                   context.Message += ex.Message;
                   
               }
           }
           context.Message += string.Format("\r\n同步结束\r\n", list.Count);
           Log.WriteLog(context.Message, "log.txt");
           if (isAllok)
           {
               File.WriteAllText("LastSysncDate.txt", date.ToString("yyyy-MM-dd hh:mm:ss.fff"));
           }
        }
    }
}
