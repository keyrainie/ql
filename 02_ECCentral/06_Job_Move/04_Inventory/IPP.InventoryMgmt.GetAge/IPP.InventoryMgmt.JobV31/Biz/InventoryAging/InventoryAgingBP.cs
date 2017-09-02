using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Dac.InventoryAging;
using IPP.InventoryMgmt.JobV31.BusinessEntities.InventoryAging;
namespace IPP.InventoryMgmt.JobV31.Biz.InventoryAge
{
    public class InventoryAgeBP
    {

        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        /// <summary>
        /// 一次获取数据的最大数量
        /// </summary>
        private static int Batch;

        //库龄同步
        public static void SolveInventoryAge(JobContext jobContext)
        {
            //日志文件
            BizLogFile = jobContext.Properties["BizLog"];
           
            //一次最大提取数
            Batch =Convert.ToInt32(ConfigurationManager.AppSettings["AgingBatch"]);

            //总提取数量
            int totalGet = 0;
            //总更新数量
            int totalInsert=0;
            //开始提取的行数
            int startNumber = 0;
            //循环次数
            int loop = 1;

            //插入失败的记录
            List<InventoryAgeEntity> erroInventoryList=new List<InventoryAgeEntity>();

            //重试成功的记录
            int retrySuccess = 0;

            DateTime? inventoryLastDate = InventoryAgeDA.GetInventoryLastDate();
            DateTime? inventoryDate = InventoryAgeDA.GetInventoryAgeDate(inventoryLastDate);
            if (inventoryDate == null)
            {
                WriteLog("无同步数据！");
                return;
            }

            //得到当天记录总数
            int firstCount = InventoryAgeDA.totalAgeCount(inventoryDate);
            while (true)
            {
                Thread.Sleep(1000);
                int lastCount = InventoryAgeDA.totalAgeCount(inventoryDate);
                if (lastCount == firstCount)
                {
                    break;
                }
                else
                {
                    firstCount = lastCount;
                }
            }
            
            if (firstCount == 0)
            {
                WriteLog("无同步数据！");
                return;
            }

            WriteLog(string.Format("同步开始,{0}条数据等待同步", firstCount));


            //清空上次同步数据
            InventoryAgeDA.ClearInventoryAge();

            while (true)
            {
                //获取本次需要同步的数据
                List<InventoryAgeEntity> inventoryList = InventoryAgeDA.GetInventoryAgeData(startNumber, Batch, inventoryDate);

                //没有数据时退出（已同步完毕）
                if (inventoryList.Count == 0)
                {
                    break;
                }
                totalGet += inventoryList.Count;

                //导入
                for (int i = 0; i < inventoryList.Count; i++)
                {
                    int affected=InventoryAgeDA.InsertInventoryAge(inventoryList[i]);
                    if (affected > 0)
                    {
                        totalInsert += 1;
                        
                    }
                    else
                    {
                        erroInventoryList.Add(inventoryList[i]);
                    }
                }
                //开始行数改变
                startNumber += Batch;
                Console.WriteLine(string.Format("第{0}轮结束,共同步{1}条数据", loop,totalInsert));

                loop += 1;
              
            }

            if (erroInventoryList.Count > 0)
            {
                retrySuccess = InsertRetry(erroInventoryList);
            }

            WriteLog(string.Format("本次共获取{0}条，同步{1}条数据！", totalGet, totalInsert + retrySuccess));


        }

        /// <summary>
        /// 重试插入
        /// </summary>
        private static int InsertRetry(List<InventoryAgeEntity> inventoryList)
        {
            int retryTimes = 1;
            int success = 0;

            while (inventoryList.Count > 0 && retryTimes <= 50)
            {
                for (int i = inventoryList.Count-1; i >= 0; i--)
                {
                    int affected = InventoryAgeDA.InsertInventoryAge(inventoryList[i]);

                    if (affected > 0)
                    {
                        Console.WriteLine(string.Format("{0}重试同步成功", inventoryList[i].Item_Key));
                        success += 1;
                        inventoryList.Remove(inventoryList[i]);
                    }
                }
  
                retryTimes += 1;
            }

            if (inventoryList.Count > 0)
            {
                for(int i=0;i<inventoryList.Count;i++)
                {
                    WriteLog(string.Format("{0}{1}{2}无法插入！", inventoryList[i].InventoryDate, inventoryList[i].Item_Key, inventoryList[i].VendorNumber));
                }
            }

            return success;
        }


        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }


    }
}
