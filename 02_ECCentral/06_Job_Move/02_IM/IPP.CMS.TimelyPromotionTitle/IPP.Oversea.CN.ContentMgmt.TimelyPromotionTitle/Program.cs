using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.ExceptionBase;
using System.Data;
using IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle.DataAccess;
using IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle.BizProcess;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle
{
    class Program : IJobAction
    {
        private static TxtFileLogger logger = LoggerManager.GetLogger();
        static void Main(string[] args)
        {

            try
            {
                logger.WriteLog(DateTime.Now + "****************************job start ********************************");
                RunNowActive();
                RunNowDisable();
                logger.WriteLog(DateTime.Now + "***************************job end****************************\r\n");
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.ToString());
                logger.WriteLog("时效促销语JOB处理完成！");
            }
        }
        /// <summary>
        /// 1.从Product_PromotionTitle 表取类型不为Normal, Status为等待,并且当前时间大于等于StartTime并且小于EndTime的数据,执行更新PromotionTitle, ProductName操作,把Status更新为有效;
        /// 即 if(UPPER([Type])='COUNTDOWN' AND [Status]='O' AND GETDATE()>=BeginDate AND GETDATE()〈Enddate)
        /// {  更新PromotionTitle;更新ProductName; Status=A; }
        /// </summary>
        private static void RunNowActive()
        {

            //  logger.WriteLog("-------更新有效促销语 Start---------");
            DataTable dt = null;
            dt = PromotionTitleDA.GetNowActiveSQL();
            int intCount = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        logger.WriteLog(DateTime.Now + "ProductSysNo:" + dr["ProductSysNo"].ToString() + ";  SysNo:" + dr["SysNo"].ToString() + ";  Status:O→A;  BeginDate:" + dr["BeginDate"].ToString() + ";  EndDate:" + dr["Enddate"].ToString() + ";");

                        PromotionTitleDA.UpdatePromotionTitleStatus(Convert.ToInt32(dr["SysNo"]), "A");
                        PromotionTitleDA.UpdatePromotionTitleAndPromotionName(Convert.ToInt32(dr["ProductSysNo"]), dr["PromotionTitle"].ToString());
                        intCount++;
                        //   logger.WriteLog("更新成功！");
                        //logger.WriteLog("**********************************************");
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("更新失败！错误信息如下：");
                        logger.WriteLog(ex.ToString());
                        //logger.WriteLog("**********************************************");
                        continue;
                    }
                }
            }
            logger.WriteLog("共更新成功有效记录" + intCount + "条！");
            //  logger.WriteLog("-------更新有效促销语 End---------");
        }
        /// <summary>
        /// 2.从Product_PromotionTitle 表取取类型不为Normal, Status为有效中或者等待,并且当前时间大于等于EndTime的数据,执行更新PromotionTitle, ProductName操作,把Status更新为失效;
        /// 即 if(Type!= Normal && (Status==O|| Status==A) && CurrentTime>= EndTime)
        /// {  更新PromotionTitle;更新ProductName; Status=D; }
        /// </summary>
        private static void RunNowDisable()
        {
            //  logger.WriteLog("-------更新过期促销语 Start---------");
            DataTable dt = null;
            string strNormalPromotionTitle = null;
            dt = PromotionTitleDA.GetNowDisableSQL();
            int intCount = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        logger.WriteLog(DateTime.Now + "ProductSysNo:" + dr["ProductSysNo"].ToString() + ";  SysNo:" + dr["SysNo"].ToString() + ";  Status:" + dr["Status"].ToString() + "→D;  BeginDate:" + dr["BeginDate"].ToString() + ";  EndDate:" + dr["Enddate"].ToString() + ";");

                        PromotionTitleDA.UpdatePromotionTitleStatus(Convert.ToInt32(dr["SysNo"]), "D");
                        strNormalPromotionTitle = PromotionTitleDA.GetNormalPromotionTitle(Convert.ToInt32(dr["ProductSysNo"]));
                        if (strNormalPromotionTitle != null)
                        {
                            PromotionTitleDA.UpdatePromotionTitleAndPromotionName(Convert.ToInt32(dr["ProductSysNo"]), strNormalPromotionTitle);
                        }
                        intCount++;
                        //  logger.WriteLog("更新成功！");
                        // logger.WriteLog("**********************************************");
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("更新失败！错误信息如下：");
                        logger.WriteLog(ex.ToString());
                        //logger.WriteLog("**********************************************");
                        continue;
                    }
                }
            }
            logger.WriteLog("共更新成功过期记录" + intCount + "条！");
            //  logger.WriteLog("-------更新过期促销语 End---------");
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            logger.WriteLog(DateTime.Now + "****************************job start ********************************");
            RunNowActive();
            RunNowDisable();
            logger.WriteLog(DateTime.Now + "***************************job end****************************\r\n");
        }

        #endregion
    }
}
