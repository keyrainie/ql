using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Common;
using System.Xml.Serialization;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using System.IO;
using IPP.InventoryMgmt.JobV31.DataAccess;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.InventoryMgmt.JobV31.BIZ
{
    public class ThirdPartInventoryQtyBP
    {
        private string LogFile;
        private StringBuilder LogList = new StringBuilder();
        private int p;
        private CommonConst Common;
        public void Run(JobContext context)
        {
            Common = Common ?? new CommonConst();

            LogList.Remove(0, LogList.Length);

            LogFile = context.Properties["BizLog"];

            WriteLog("第三方库存同步Job已启动。", true);

            WriteLog("正在检索本次需要同步库存的数据信息……", true);

            try
            {
                ThirdPartInventoryBPBase bp = new ThirdPartInventoryBP(Common);

                WriteLog(string.Format("本次共检测到{0}条需同步的数据。", bp.Count), true);

                bp.OnRunningBefor += WriteDataLog;
                bp.OnRunningAfter += ModifyLocalQty;
                bp.OnError += WriteErrorLog;

                bp.Start();

            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                WriteLog(string.Format("{0} \r\n {1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                WriteLog("本次同步结束，Job退出。\r\n\r\n", true);
                EndLog();
            }
        }

        #region 私有方法

        private void WriteDataLog(object sender, ThirdPartInventoryArgs args)
        {
            if (args.ThirdPartInventoryList.Count == 0)
            {
                ThirdPartInventoryBPBase bp = sender as ThirdPartInventoryBPBase;
                if (bp != null)
                {
                    bp.Abort();
                    return;
                }
            }
            p++;
            WriteLog(string.Format("正在同步第{0}批数据，本批次数据量：{1}条……", p, args.ThirdPartInventoryList.Count), true);
            StringBuilder sb = new StringBuilder();
            sb.Append("ProductMappingSysno_ProductSysNo_AlamQty[");
            foreach (ThirdPartInventoryEntity entity in args.ThirdPartInventoryList)
            {
                sb.AppendFormat("{0}_{1}_{2} ", entity.ProductMappingSysno, entity.ProductSysNo, entity.InventoryAlamQty ?? Common.InventoryAlarmQty);
            }
            sb.Append("]\r\n");
            WriteLog(sb.ToString());

        }

        private void ModifyLocalQty(object sender, ThirdPartInventoryArgs args)
        {
            List<ThirdPartInventoryEntity> list = args.ThirdPartInventoryList;
            foreach (ThirdPartInventoryEntity entity in list)
            {
                int synInventoryQty = Addapter.CalculateInventoryQty.CalculateQty(entity);
                int inventoryQty = entity.InventoryOnlineQty - (entity.SynInventoryQty + entity.OldInventoryAlamQty);
                if (entity.InventoryAlamQty.HasValue)//如果库存预警为空，则证明该数据尚未被初始化
                {
                    ThirdPartInventoryDA.Modify(inventoryQty, synInventoryQty, entity.ProductMappingSysno, Common);
                }
                else//初始化数据
                {
                    //synInventoryQty += 1;
                    ThirdPartInventoryDA.Insert(inventoryQty, synInventoryQty, entity.ProductMappingSysno, Common);
                }
            }
            WriteLog("本批次数据同步成功。\r\n", true);
        }

        private void WriteErrorLog(object sender, ThirdPartInventoryErrorArgs args)
        {
            ExceptionHelper.HandleException(args.Exception);
            string error = string.Format("{0} \r\n {1}", args.Exception.Message, args.Exception.StackTrace);
            List<ThirdPartInventoryEntity> list = args.ThirdPartInventoryList;
            XmlSerializer ser = new XmlSerializer(typeof(List<ThirdPartInventoryEntity>));
            MemoryStream stream = new MemoryStream();
            ser.Serialize(stream, list);
            string xmlString = Encoding.UTF8.GetString(stream.GetBuffer());
            stream.Dispose();
            WriteLog(string.Format("出错数据：\r\n{0}", xmlString));
            WriteLog(string.Format("{0}\r\n", error), true);
            if (Common.ThirdPartSynType == SynType.Queue)
            {
                ThirdPartInventoryBPBase bp = sender as ThirdPartInventoryBPBase;
                if (bp != null)
                {
                    bp.Abort();
                }
            }
        }

        private void WriteLine(string mes)
        {
            Console.WriteLine(mes);
        }

        private void WriteLog(string mes, bool output)
        {
            if (output)
            {
                WriteLine(mes);
            }
            LogList.AppendLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), mes));
            if (LogList.Capacity > 5000)
            {
                EndLog();
            }
        }

        private void WriteLog(string mes)
        {
            WriteLog(mes, false);
        }
        private void EndLog()
        {
            //2011-11-3 Modify 去掉物理log记录
            //LogBP.WriteLog(LogList.ToString(), LogFile);
            LogList = LogList.Remove(0, LogList.Length);
        }
        #endregion
    }
}
