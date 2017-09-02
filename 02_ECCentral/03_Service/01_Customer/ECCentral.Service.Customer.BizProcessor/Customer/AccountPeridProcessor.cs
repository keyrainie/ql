using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(AccountPeridProcessor))]
    public class AccountPeridProcessor
    {
        private IAccountPeriodDA da = ObjectFactory<IAccountPeriodDA>.Instance;
        public virtual AccountPeriodInfo AdjustCollectionPeriodAndRating(AccountPeriodInfo entity)
        {
            if (!entity.AvailableCreditLimit.HasValue)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerInfo", "AdjustCollectionPeriodAndRating_CreditLimit_NoNull"));
            }
            AccountPeriodInfo accountInfo = GetCustomerAccountPeriod(entity.CustomerSysNo.Value);
            if (accountInfo != null)
            {
                entity.CustomerSysNo = accountInfo.CustomerSysNo;

                decimal availableCreditLimit = 0;

                if (!accountInfo.AvailableCreditLimit.HasValue)
                {
                    accountInfo.AvailableCreditLimit = 0;
                }
                availableCreditLimit = accountInfo.AvailableCreditLimit.Value + entity.AvailableCreditLimit.Value;
                if (availableCreditLimit < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CustomerInfo", "AdjustCollectionPeriodAndRating_CreditLimit_Negative"));
                }
                entity.AvailableCreditLimit = availableCreditLimit;
            }
            da.UpdateAvailableCreditLimit(entity.CustomerSysNo.Value,entity.AvailableCreditLimit.Value);
            return entity;
        }
        /// <summary>
        /// 直接设置账期现在金额
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="receivableAmount"></param>
        /// <returns></returns>
        public virtual void SetCreditLimit(int customerSysNo, decimal AvailableCreditLimit)
        {
            da.UpdateAvailableCreditLimit(customerSysNo, AvailableCreditLimit);
        }
        public virtual void AddCreditLimit(int customerSysNo, decimal receivableAmount)
        {
            AccountPeriodInfo entity = da.GetAccountPeriodInfoByCustomerSysNo(customerSysNo);
            if (entity != null)
            {
                entity.AvailableCreditLimit += receivableAmount;

                if (entity.AvailableCreditLimit < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CustomerInfo", "SetCreditLimit_CreditLimit_Negative"));
                }
            }
            SetCreditLimit(entity.CustomerSysNo.Value, entity.AvailableCreditLimit.Value);
        }
        /// <summary>
        /// 获取用户的账期信息
        /// </summary>
        /// <param name="csutomerSysNo"></param>
        /// <returns></returns>
        public virtual AccountPeriodInfo GetCustomerAccountPeriod(int csutomerSysNo)
        {
            return da.GetAccountPeriodInfoByCustomerSysNo(csutomerSysNo);
        }
        public virtual void CreateAccountPeriodInfo(AccountPeriodInfo entity)
        {
            da.CreateAccountPeriodInfo(entity);
        }
        public virtual void UpdateAccountPeriodInfo(AccountPeriodInfo entity)
        {
            da.UpdateAccountPeriodInfo(entity);

            ExternalDomainBroker.CreateOperationLog("SetCreditLimit Customer"
                                           , BizLogType.Basic_Customer_SetCreditLimit
                                           , entity.CustomerSysNo.Value
                                           , "8601");
        }
    }
}

