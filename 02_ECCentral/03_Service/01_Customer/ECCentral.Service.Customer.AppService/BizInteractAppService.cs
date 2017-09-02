using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(ICustomerBizInteract))]
    public class BizInteractAppService : ICustomerBizInteract
    {
        #region ICustomerBizInteract Members
        public virtual bool CustomerIsExists(int customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.IsExists(customerSysNo);
        }
        public virtual CustomerInfo GetCustomerInfo(int customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.GetCustomerBySysNo(customerSysNo);
        }

        public virtual CustomerBasicInfo GetCustomerBasicInfo(int customerSysNo)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(customerSysNo);
        }

        public virtual List<CustomerBasicInfo> GetCustomerBasicInfo(List<int> customerSysNoList)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNos(customerSysNoList);
        }

        public virtual void AdjustCustomerCreditLimit(int customerSysNo, decimal receivableAmount)
        {
            ObjectFactory<AccountPeridProcessor>.Instance.AddCreditLimit(customerSysNo, receivableAmount);
        }

        public virtual void SetCustomerCreditLimit(int customerSysNo, decimal limitAmount)
        {
            ObjectFactory<AccountPeridProcessor>.Instance.SetCreditLimit(customerSysNo, limitAmount);
        }

        public virtual void UpdateCustomerOrderedAmount(int customerSysNo, decimal orderedAmt)
        {
            ObjectFactory<CustomerProcessor>.Instance.UpdateOrderedAmount(customerSysNo, orderedAmt);
        }

        public virtual int CreateAdjustPointRequest(int customerSysNo, int soSysNo, string adjustAccount, int point, int logType, List<int> productSysNoList, string memo)
        {
            throw new NotImplementedException();
        }

        public virtual void SetMaliceCustomer(int customerSysNo, bool isMalice, string memo, int? SoSysNo)
        {
            CustomerOperateLog log = new CustomerOperateLog();
            log.Memo = memo;
            log.SOSysNo = SoSysNo;
            ObjectFactory<CustomerProcessor>.Instance.MaintainMaliceUser(customerSysNo, isMalice, log);
        }

        public virtual void UpdateCustomerShippingInfo(ShippingAddressInfo shippingInfo)
        {
            ObjectFactory<ShippingAddressProcessor>.Instance.UpdateShippingAddress(shippingInfo);
        }

        public virtual List<CustomerRight> GetCustomerRight(int customerSysNo)
        {
            return ObjectFactory<CustomerRightProcessor>.Instance.LoadAllCustomerRight(customerSysNo);
        }

        public virtual bool CheckCustomerRemainingAmount(int customerSysNo, decimal paymentAmount)
        {
            return ObjectFactory<PrepayProcessor>.Instance.GetValidPrepay(customerSysNo) > paymentAmount;
        }

        public virtual void SetCustomerValueAddedTax(int VATSysNo, int customerSysNo, string bankAcct, string companyName, string companyAddress, string companyPhone, string taxAccount, bool isDefault, bool IsUpdate)
        {
            ValueAddedTaxInfo entity = new ValueAddedTaxInfo();
            entity.SysNo = VATSysNo;
            entity.CustomerSysNo = customerSysNo;
            entity.BankAccount = bankAcct;
            entity.CompanyName = companyName;
            entity.CompanyAddress = companyAddress;
            entity.CompanyPhone = companyPhone;
            entity.TaxNum = taxAccount;
            entity.IsDefault = isDefault;
            entity.LastEditDate = DateTime.Now;
            if (IsUpdate)
            {
                ObjectFactory<ValueAddedTaxProcessor>.Instance.UpdateValueAddedTaxInfo(entity);
            }
            else
            {
                ObjectFactory<ValueAddedTaxProcessor>.Instance.CreateValueAddedTaxInfo(entity);
            }
        }

        public virtual void AdjustCustomerExperience(int customerSysNo, decimal experience, ExperienceLogType type, string memo)
        {
            CustomerExperienceLog entity = new CustomerExperienceLog();
            entity.CustomerSysNo = customerSysNo;
            entity.Amount = experience;
            entity.Memo = memo;
            entity.Type = type;
            ObjectFactory<ExperienceProcessor>.Instance.Adjust(entity);
        }

        public virtual void CloseCallsEvents(CallingReferenceType ReferenceType, int ReferenceSysNo, string note)
        {
            //ObjectFactory<CallsEventsProcessor>.Instance.CloseCallsEvents(ReferenceType, ReferenceSysNo, note);
        }

        public virtual void UpatePointExpiringDate(int customerSysNo, DateTime pointExpiringDate, string memo)
        {
            ObjectFactory<PointProcessor>.Instance.UpateExpiringDate(customerSysNo, pointExpiringDate);
        }

        public virtual int GetCustomerPointAddRequestStatus(int requestSysNo)
        {
            throw new NotImplementedException();
        }

        public virtual void AbandonAdjustPointRequest(int requestSysNo)
        {
            throw new NotImplementedException();
        }

        #endregion ICustomerBizInteract Members

        public virtual CustomerRank GetCustomerRank(int customerSysNo)
        {
            return ObjectFactory<RankProcessor>.Instance.GetRank(customerSysNo);
        }

        public virtual void AdjustPointPreCheck(AdjustPointRequest info)
        {
            throw new NotImplementedException();
        }

        public virtual void AdjustPoint(AdjustPointRequest info)
        {
            ObjectFactory<PointProcessor>.Instance.Adjust(info);
        }

        public virtual void AdjustPrePayPreCheck(CustomerPrepayLog info)
        {
            throw new NotImplementedException();
        }

        public virtual void AdjustPrePay(CustomerPrepayLog info)
        {
            ObjectFactory<PrepayProcessor>.Instance.Adjust(info);
        }

        public virtual void AdjustCreditLimitPreCheck(int customerSysNo, decimal amount)
        {
        }

        /// <summary>
        /// 返还订单的奖品 订单作废时需返还奖品
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="soSysNo"></param>
        public virtual void ReturnGiftForSO(int soSysNo)
        {
            ObjectFactory<GiftProcessor>.Instance.ReturnGiftForSO(soSysNo);
        }

        public virtual int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList)
        {
            return ObjectFactory<PointProcessor>.Instance.GetPriceprotectPoint(soSysNo, productSysNoList);
        }
        /// <summary>
        /// 取得短信内容
        /// </summary>
        /// <param name="webChannelID">渠道ID</param>
        /// <param name="languageCode">语言Code</param>
        /// <param name="shipTypeSysNo">运输方式系统编号</param>
        /// <param name="Type">短信类型</param>
        /// <returns></returns>
        public virtual string GetSMSContent(string webChannelID, string languageCode, int shipTypeSysNo, SMSType Type)
        {
            return ObjectFactory<ShipTypeSMSProcessor>.Instance.GetSMSContent(webChannelID, languageCode, shipTypeSysNo, Type);
        }

        public decimal GetPointToMoneyRatio()
        {
            return decimal.Parse(AppSettingManager.GetSetting(CustomerConst.DomainName, CustomerConst.Key_PointToMonetyRatio));
        }

        #region point for order


        public virtual void SplitSOPointLog(SOBaseInfo master, List<SOBaseInfo> subSoList)
        {
            ObjectFactory<PointProcessor>.Instance.SplitSOPointLog(master.CustomerSysNo.Value, master, subSoList);
        }

        public virtual void CancelSplitSOPointLog(SOBaseInfo master, List<SOBaseInfo> subSoList)
        {
            ObjectFactory<PointProcessor>.Instance.CancelSplitSOPointLog(master.CustomerSysNo.Value, master, subSoList);
        }

        #endregion

        #region ICustomerBizInteract Members

        /// <summary>
        /// 领取奖品
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="soSysNo"></param>
        public virtual void GetGift(int customerSysNo, int productSysNo, int soSysNo)
        {
            ObjectFactory<GiftProcessor>.Instance.GetGift(customerSysNo, productSysNo, soSysNo);
        }

        #endregion

        #region ICustomerBizInteract Members


        public virtual List<CustomerBasicInfo> GetSystemAccount(string webChannelID)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetSystemAccount(webChannelID);
        }

        public virtual int GetCustomerVaildScore(int customerSysNo)
        {
            return ObjectFactory<PointProcessor>.Instance.GetValidPoint(customerSysNo);
        }

        #endregion

        #region CSTool
        public List<FPCheck> GetFPCheckList(string companyCode)
        {
            return ObjectFactory<FPCheckProcessor>.Instance.GetFPCheckList(companyCode);
        }

        public List<OrderCheckMaster> GetOrderCheckList(string companyCode)
        {
            return ObjectFactory<OrderCheckMasterProcessor>.Instance.GetOrderCheckList(companyCode);
        }
        #endregion

        /// <summary>
        /// 获取所有恶意用户
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<CustomerInfo> GetMalevolenceCustomerList(string companyCode)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetMalevolenceCustomerList(companyCode);
        }
       
        /// <summary>
        /// 获取补偿退款单的状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public virtual RefundAdjustStatus? GetRefundAdjustStatus (int SysNo)
        {
            return ObjectFactory<RefundAdjustProcessor>.Instance.GetRefundAdjustStatus(SysNo);
        }

        public virtual string SendSMS(string cellPhone, string message)
        {
            string errorMsg;
            ObjectFactory<SMSProcessor>.Instance.SendByCellphone(cellPhone, message, out errorMsg);
            return errorMsg;
        }


        public void AuditRefundAdjust(int SysNo, RefundAdjustStatus Status, int? RefundUserSysNo, DateTime? AuditTime)
        {
            throw new NotImplementedException();
        }
    }
}