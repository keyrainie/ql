using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    public static class ExternalDomainBroker
    {
        #region Common domain
        /// <summary>
        /// 根据codeSysNo查询ReasonCode路径
        /// </summary>
        /// <param name="reasonCodeSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }
        /// <summary>
        /// 创建操作日志，提供统一的日志记录服务
        /// </summary>
        /// <param name="note"></param>
        /// <param name="bizLogType"></param>
        /// <param name="ticketSysNo"></param>
        /// <param name="companyCode"></param>
        public static void CreateOperationLog(string note, BizEntity.Common.BizLogType bizLogType, int ticketSysNo, string companyCode)
        {
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note, bizLogType, ticketSysNo, companyCode);
        }

        /// <summary>
        ///  根据key获取IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetSystemConfigurationValue(string key, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetSystemConfigurationValue(key, companyCode);
        }
        /// <summary>
        /// 根据系统用户的编号获得系统用户信息 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfoBySysNo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(sysNo);
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="cellphone"></param>
        /// <param name="message"></param>
        /// <param name="sMSPriority"></param>
        public static void SendSMS(string cellphone, string message, SMSPriority sMSPriority)
        {
            ObjectFactory<ICommonBizInteract>.Instance.SendSMS(cellphone, message, sMSPriority);
        }
        public static List<DateTime> GetHoliday(string blockedService, string CompanyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetHolidayList(blockedService, CompanyCode);
        }
        #endregion

        #region SO domain
        /// <summary>
        /// 根据订单编号取得订单所有信息
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        public static BizEntity.SO.SOInfo GetSOInfo(int SOSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(SOSysNo);
        }

        /// <summary>
        /// 添加订单投诉
        /// </summary>
        /// <param name="complaintInfo"></param>
        /// <returns></returns>
        public static BizEntity.SO.SOComplaintCotentInfo AddComplain(BizEntity.SO.SOComplaintCotentInfo complaintInfo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.AddComplain(complaintInfo);
        }
        /// <summary>
        /// 根据订单编号查检订单是否存在
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static bool ExistSO(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.ExistSO(soSysNo);
        }

        /// <summary>
        /// 根据订单编号取得订单主要信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static BizEntity.SO.SOBaseInfo GetSOBaseInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfo(soSysNo);
        }

        /// <summary>
        /// 根据订单编号取得订单收货人手机
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static string GetSOReceiverPhone(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOReceiverPhone(soSysNo);
        }
        #endregion

        #region RMA domain

        /// <summary>
        /// 创建RMA跟进信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static BizEntity.RMA.InternalMemoInfo CreateRMATracking(BizEntity.RMA.InternalMemoInfo request)
        {
            return ObjectFactory<IRMABizInteract>.Instance.CreateRMATracking(request);
        }

        /// <summary>
        /// 是否物流拒收
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static int GetAutoRMARefundCountBySOSysNo(int soSysNo)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetAutoRMARefundCountBySOSysNo(soSysNo);
        }

        #endregion

        #region Invoice domain
        /// <summary>
        /// 根据单据类型和单据编号取得已经确认的销售收款单
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <param name="sOIncomeOrderType"></param>
        /// <returns></returns>
        public static BizEntity.Invoice.SOIncomeInfo GetConfirmedSOIncomeInfo(int orderSysNo, BizEntity.Invoice.SOIncomeOrderType sOIncomeOrderType)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetConfirmedSOIncome(orderSysNo, sOIncomeOrderType);
        }

        /// <summary>
        /// 创建补偿退款单对应的销售退款单
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        public static BizEntity.Invoice.SOIncomeRefundInfo CreateIncomeRefund(BizEntity.Invoice.SOIncomeRefundInfo refund)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncomeRefund(refund);
        }

        /// <summary>
        /// 创建负的财务收款单
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public static SOIncomeInfo CreateNegative(SOIncomeRefundInfo refundInfo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateNegative(refundInfo);
        }

        /// <summary>
        /// 调整积分有效期
        /// </summary>
        /// <param name="obtainSysNo"></param>
        /// <param name="expiredDate"></param>
        public static void UpdatePointExpiringDate(int obtainSysNo, DateTime expiredDate)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.UpdatePointExpiringDate(obtainSysNo, expiredDate);
        }

        public static object AdjustPoint(AdjustPointRequest adjustInfo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.AdjustPoint(adjustInfo);
        }

        public static object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.SplitSOPointLog(customerSysNo, master, subSoList);
        }

        public static object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CancelSplitSOPointLog(customerSysNo, master, subSoList);
        }
        #endregion

        #region IM domain

        public static ProductInfo GetProductInfo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
        }

        #endregion

    }
}
