using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvoiceMgmt.JobV31.Dac;
using InvoiceMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.BizEntity.Customer;

namespace InvoiceMgmt.JobV31.Biz
{
    public class VIPCustomerPresentedPointsBP
    {
        /// <summary>
        /// 获取VIP卡用户年购10000元的VIP客户送500积分
        /// </summary>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        public static string PresentedPoints(JobContext jobContext)
        {
            string resultMessage = string.Empty;                 
            List<VIPCustomerEntity> VIPCustomerEntityList = VIPCustomerPresentedPointsDA.GetVipCustomerOfNeedPresentedPointsList();
            if (VIPCustomerEntityList == null || VIPCustomerEntityList.Count==0)
            {
                resultMessage = "没有符合赠送积分条件的客户(赠送积分条件：VIP卡用户年购满10000元)";
            }
            else
            {                  
                //VIP卡用户年购10000元添加500积分
                int flag = VIPCustomerEntityList.Count;
                foreach (VIPCustomerEntity vipItem in VIPCustomerEntityList)
                {                 
                    try
                    {
                        //赠送积分校验
                        PresentedCustomerPointPrecheck(vipItem.Sysno, 500);
                        //赠送积分
                        PresentedCustomerPoint(vipItem.Sysno, 500);
                        //赠送完成后更新这些客户状态为 已赠送积分状态
                        VIPCustomerPresentedPointsDA.UpdateVIPCustomerPresentedPointsStatus(vipItem.Sysno);
                    }
                    catch (Exception ex)
                    {
                        resultMessage +="客户编号：" +vipItem.Sysno +"赠送积分未成功："+ ex.Message + "\n";
                        WriteErrorLog(jobContext, ex, vipItem.Sysno);
                        flag--;
                    }
                }
                if (VIPCustomerEntityList.Count == flag)//全部客户 赠送成功
                {
                    resultMessage = "赠送积分结束(需要赠送客户数量：" + VIPCustomerEntityList.Count + ",成功赠送客户数量：" + VIPCustomerEntityList.Count + "位)";
                }
                else
                {
                    if (flag==0)//全部客户  赠送失败
                    {
                        resultMessage += "\n\n赠送积分结束(需要赠送客户数量：" + VIPCustomerEntityList.Count + ",成功赠送客户数量：" + flag + "位)";
                    }
                    else//部分客户赠送成功
                    {
                        resultMessage += "\n\n赠送积分结束(需要赠送客户数量：" + VIPCustomerEntityList.Count + ",成功赠送客户数量：" + (VIPCustomerEntityList.Count - flag) + "位)";
                    }
                }
            }             
            return resultMessage;
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="jobContext"></param>
        /// <param name="ex"></param>
        private static void WriteErrorLog(JobContext jobContext, Exception ex, int customerSysno)
        {
            jobContext.Message += "客户编号：" + customerSysno + "赠送积分产生异常" + ex.Message + "\n";
        }

        #region 赠送积分 ----- VIP卡用户年购满10000元送500积分 ----

        /// <summary>
        ///  赠送客户500积分校验
        /// </summary>
        /// <param name="customerSysNo">客户系统编号</param>
        /// <param name="pointPay">赠送积分数</param>
        public static void PresentedCustomerPoint(int customerSysNo, int pointPay)
        {
            if (pointPay == 0)
            {
                return;
            }
            AdjustPointRequestInfo pointRequest = new AdjustPointRequestInfo
            {
                CustomerSysNo = customerSysNo,
                Point = pointPay,
                PointLogType = (int)AdjustPointType.SalesGivePoints,//促销活动送积分（枚举值：25）
                OptType = 0,   //OperationType=0 添加积分，当OperationType=1 积分撤销
                SOSysNo = null,
                Note = "VIP卡用户年购满10000元送500积分",
                Memo = "VIP卡用户年购满10000元送500积分",
            };

            PresentedCustomerPoint(pointRequest);
        }

        /// <summary>
        /// 赠送客户积分
        /// </summary>
        /// <param name="pointRequest"></param>
        public static void PresentedCustomerPoint(AdjustPointRequestInfo pointRequest)
        {
            if (pointRequest == null)
            {
                return;
            }

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceMgmtRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            string relativeUrl = "/InvoiceService/Job/AdjustPoint";

            var ar = client.Create(relativeUrl, pointRequest, out error);
            var messageBuilder = new StringBuilder();
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    messageBuilder.AppendFormat(" {0} <br/>", errorItem.ErrorDescription);
                }

                throw new Exception(messageBuilder.ToString());
            }
            
        }

        /// <summary>
        /// 赠送客户积分校验
        /// </summary>
        /// <param name="customerSysNo">客户系统编号</param>
        /// <param name="pointPay">赠送积分数</param>
        public static void PresentedCustomerPointPrecheck(int customerSysNo, int pointPay)
        {
            AdjustPointRequestInfo pointRequest = new AdjustPointRequestInfo
            {
                CustomerSysNo = customerSysNo,
                Point = pointPay,
                PointLogType = (int)AdjustPointType.SalesGivePoints,//促销活动送积分（枚举值：25）
                OptType = 0,   //OperationType=0 添加积分，当OperationType=1 积分撤销；
                SOSysNo = null,
                Note = "VIP卡用户年购满10000元送500积分",
                Memo = "VIP卡用户年购满10000元送500积分",
            };

            PresentedCustomerPointPrecheck(pointRequest);
        }

        /// <summary>
        /// 赠送客户积分校验
        /// </summary>
        /// <param name="pointRequest"></param>
        public static void PresentedCustomerPointPrecheck(AdjustPointRequestInfo pointRequest)
        {
            if (pointRequest == null)
            {
                return;
            }

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceMgmtRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            string relativeUrl = "/InvoiceService/Job/AdjustPointPreCheck";

            var ar = client.Create(relativeUrl, pointRequest, out error);
            var messageBuilder = new StringBuilder();
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    messageBuilder.AppendFormat(" {0} <br/>", errorItem.ErrorDescription);
                }

                throw new Exception(messageBuilder.ToString());
            }
        }
        #endregion
    }
}
