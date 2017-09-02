using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 调用外部接口的内部通用类。将Inventory中调用外部接口的地方都集中到这里。
    /// </summary>
    public static class ExternalDomainBroker
    {
        #region 发送邮件

        /// <summary>
        /// 发送内部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        /// <param name="isAsync"></param>
        public static void SendInternalEmail(string templateID, KeyValueVariables keyValueVariables, KeyTableVariables keyTableVariables)
        {
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(null, null, null, templateID, keyValueVariables, keyTableVariables, true, true,"zh-CN");
        }

        public static void SendInternalEmail(string toAddr, string templateID, KeyValueVariables keyValueVariables, KeyTableVariables keyTableVariables)
        {
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(toAddr, null, null, templateID, keyValueVariables, keyTableVariables, true, true, "zh-CN");
        }

        /// <summary>
        /// 异步发送内部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        /// <param name="isAsync"></param>
        public static void SendInternalEmail(string templateID, KeyValueVariables keyValueVariables)
        {
            SendInternalEmail(templateID, keyValueVariables, null);
        }

        /// <summary>
        /// 异步发送外部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        public static void SendExternalEmail(string toAddress, string templateID, KeyValueVariables keyValueVariables, KeyTableVariables keyTableVariables, string languageCode)
        {
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(toAddress, templateID, keyValueVariables, keyTableVariables, languageCode);
        }

        public static void SendExternalEmail(string toAddress, string templateID, KeyValueVariables keyValueVariables, string languageCode)
        {
            SendExternalEmail(toAddress, templateID, keyValueVariables, null, languageCode);
        }

        #endregion 发送邮件

        #region IM Domain

        /// <summary>
        /// 根据商品列表取得商品编号。
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public static List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        /// <summary>
        /// 根据商品列表取得商品编号。
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public static ProductInfo GetProductInfoByProductSysNo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
        }

        #endregion IM Domain

        #region PO Domain
        /// <summary>
        /// 批量创建代销转财务记录（貌似没有被调用）
        /// </summary>
        /// <param name="consignToAcctLogInfos"></param>
        public static void BatchCreateConsignToAcctLogs(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            ObjectFactory<IPOBizInteract>.Instance.BatchCreateConsignToAcctLogs(consignToAcctLogInfos);
        }
        /// <summary>
        /// 批量创建代销转库存记录
        /// </summary>
        /// <param name="consignToAcctLogInfos"></param>
        public static void BatchCreateConsignToAcctLogsInventory(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            ObjectFactory<IPOBizInteract>.Instance.BatchCreateConsignToAcctLogsInventory(consignToAcctLogInfos);
        }
        
        #endregion

        #region MKT Domain
        /// <summary>
        /// 获取就绪/运行的限时促销计划列表
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns>就绪/运行的限时促销计划列表</returns>
        public static List<CountdownInfo> GetReadyOrRunningCountDownByProductSysNo(int productSysNo)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetReadyOrRunningCountDownByProductSysNo(productSysNo);
        }

        public static bool CheckBuyLimitAndIsNotLimitedQtyANDIsNotReservedQty(List<CountdownInfo> countDownList)
        {
            return countDownList.Find(x => x.IsLimitedQty == false && x.IsReservedQty == false) != null;
        }

        public static bool CheckBuyLimitAndIsLimitedQtyORIsReservedQty(List<CountdownInfo> countDownList)
        {
            return countDownList.Find(x => x.IsLimitedQty == true || x.IsReservedQty == true) != null;
        }
        #endregion

        #region SO Domain
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static SOInfo GetSOInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(soSysNo);
        }
        #endregion

        #region Common Domain

        private static ICommonBizInteract _commonBizInteract;
        private static ICommonBizInteract CommonBizInteract
        {
            get
            {
                _commonBizInteract = _commonBizInteract ?? ObjectFactory<ICommonBizInteract>.Instance;
                return _commonBizInteract;
            }
        }

        public static UserInfo GetUserInfo(int userSysNo)
        {
            return CommonBizInteract.GetUserInfoBySysNo(userSysNo);
        }
        
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="note"></param>
        /// <param name="logType"></param>
        /// <param name="ticketSysNo"></param>
        /// <param name="companyCode"></param>
        internal static void CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            CommonBizInteract.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        #region 取得系统配置

        /// <summary>
        /// 取得系统配置
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetSystemConfigurationValue(string key, string companyCode)
        {
            return CommonBizInteract.GetSystemConfigurationValue(key, companyCode);
        }

        /// <summary>
        /// 记录biz日志
        /// </summary>
        /// <param name="note">日志内容</param>
        /// <param name="logType">日志类型</param>
        /// <param name="ticketSysNo">单据号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>影响行数</returns>
        internal static int WriteBizLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return CommonBizInteract.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        #endregion 取得系统配置

        #endregion Common Domain

    }
}
