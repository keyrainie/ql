using System;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Dac;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class AVGDailySalesToSAPBP
    {
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;
    

        public static string Process(JobContext jobContext)
        {
            //日志文件
            BizLogFile = jobContext.Properties["BizLog"];

            //是否初始化有销售记录的商品
            string ISInitialHavaSaledRecord = jobContext.Properties["ISInitialHavaSaledRecord"];
            //是否初始化没有有销售记录的商品
            string ISInitialHavaNotSaledRecord = jobContext.Properties["ISInitialHavaNotSaledRecord"]; 

            string resultMessage = "同步成功";
            try
            {
                #region 有销售记录的商品  计算化日均销量 和可卖天数

                if (ISInitialHavaSaledRecord == "1")
                {
                    int NeedInitialProductCount = 500;
                    NeedInitialProductCount = AVGDailySalesInitialDA.GetNeedAVGDailySalesInitialData();
                    if (NeedInitialProductCount == 0)
                    {
                        resultMessage = "没有数据需要初始化";
                    }
                    else if (NeedInitialProductCount > 500)
                    {
                        int loopCount = 1;
                        loopCount = (int)Math.Ceiling((Double)(NeedInitialProductCount) / (Double)500);
                        for (int i = 0; i < loopCount; i++)
                        {
                            AVGDailySalesInitialDA.AVGDailySalesInitial(500);
                        }
                    }
                    else
                    {                   
                        AVGDailySalesInitialDA.AVGDailySalesInitial(NeedInitialProductCount);
                    }
                }

                #endregion

                #region 没有销售记录 但是是 上架销售 和上架展示的商品 计算化日均销量 和可卖天数

                if (ISInitialHavaNotSaledRecord == "1")
                 {
                        int NeedInitialNotSaledProductCount = 500;
                        NeedInitialNotSaledProductCount = AVGDailySalesInitialDA.GetNeedAVGDailySalesInitialDataOfNotSaledRecord();
                        if (NeedInitialNotSaledProductCount == 0)
                        {
                            resultMessage = "没有数据需要初始化";
                        }
                        else if (NeedInitialNotSaledProductCount > 500)
                        {
                            int loopCount = 1;
                            loopCount = (int)Math.Ceiling((Double)(NeedInitialNotSaledProductCount) / (Double)500);
                            for (int i = 0; i < loopCount; i++)
                            {
                                AVGDailySalesInitialDA.AVGDailySalesInitialOfNotSaledRecord(500);
                            }
                        }
                        else
                        {
                            AVGDailySalesInitialDA.AVGDailySalesInitialOfNotSaledRecord(NeedInitialNotSaledProductCount);
                        }
                }

                #endregion
            }
            catch(Exception ex)
            {
                resultMessage = ex.Message;
                WriteLog(string.Format("同步失败:{0}", ex.Message));
                WriteErrorLog(jobContext, ex);
            }
            return resultMessage;
        }
 
        /// <summary>
        /// 业务日志
        /// </summary>
        /// <param name="content"></param>
        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

        /// <summary>
        /// Job异常日志
        /// </summary>
        /// <param name="jobContext"></param>
        /// <param name="ex"></param>
        private static void WriteErrorLog(JobContext jobContext, Exception ex)
        {
            jobContext.Message += "同步产生异常" + ex.Message + "\n";
        } 

    }
}
