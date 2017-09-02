using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Customer;
using ECommerce.Entity.Customer;
using ECommerce.Entity.SO;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Service.Customer
{
    public class CustomerService
    {
        public static void UpdateCustomerOrderedAmt(int customerSysNo, decimal orderedAmt)
        {
            CustomerDA.AdjustOrderedAmount(customerSysNo, orderedAmt);
        }

        public static CustomerBasicInfo GetCustomerInfo(int customerSysNo)
        {
            CustomerBasicInfo customerInfo = CustomerDA.GetCustomerInfo(customerSysNo);
            return customerInfo;
        }

        public static decimal GetPointToMoneyRatio()
        {
            return decimal.Parse(AppSettingManager.GetSetting("Product", "Product_PointExhangeRate"));
        }


        public static void AdjustPoint(AdjustPointRequest adujstInfo, int userSysNo)
        {
            //1.做检查
            if (!adujstInfo.CustomerSysNo.HasValue || adujstInfo.CustomerSysNo.Value == 0)
            {
                throw new ArgumentException(L("用户编号不能为空"));
            }
            if (string.IsNullOrEmpty(adujstInfo.Source))
            {
                throw new BusinessException(L("积分调整来源系统不能为空"));
            }
            if (string.IsNullOrEmpty(adujstInfo.Memo))
            {
                throw new BusinessException(L("积分调整原因不能为空"));
            }
            CustomerDA.AdjustPoint(adujstInfo, userSysNo);
        }

        public static void AdjustPrePay(CustomerPrepayLog adjustInfo)
        {
            CustomerBasicInfo original = GetCustomerInfo(adjustInfo.CustomerSysNo.Value);
            using (ITransaction tran = TransactionManager.Create())
            {
                //1.更新余额
                CustomerDA.UpdatePrepay(adjustInfo.CustomerSysNo.Value, adjustInfo.AdjustAmount.Value);
                //2.记录业务日志
                CustomerDA.CreatePrepayLog(adjustInfo);
                tran.Complete();
            }
        }

        public static void AdjustCustomerExperience(int customerSysNo, decimal experience, ExperienceLogType type, string memo)
        {
            CustomerExperienceLog entity = new CustomerExperienceLog();
            entity.CustomerSysNo = customerSysNo;
            entity.Amount = experience;
            entity.Memo = memo;
            entity.Type = type;


            CustomerBasicInfo customer = GetCustomerInfo(customerSysNo);
            var totalExperience = customer.TotalExperience.Value;
            totalExperience = totalExperience + entity.Amount.Value;
            if (totalExperience < 0)
            {
                throw new BusinessException("顾客的最终经验值必须为正值");
            }

            CustomerDA.UpdateExperience(entity.CustomerSysNo.Value, totalExperience);

            CustomerExperienceLog cuslog = new CustomerExperienceLog();
            cuslog.CustomerSysNo = entity.CustomerSysNo;
            cuslog.Amount = entity.Amount;
            cuslog.Memo = string.Format("（用户系统号:{0};原值:{1}){2}",
                 entity.CustomerSysNo, totalExperience - cuslog.Amount, entity.Memo);
            cuslog.Type = entity.Type;
            CustomerDA.InsertExperienceLog(cuslog);

            CustomerDA.SetRank(entity.CustomerSysNo.Value);
        }


        /// <summary>
        /// 给款到发货的客户加积分
        /// </summary>
        /// <param name="soInfo"></param>
        public static void AddPointForCustomer(SOInfo soInfo)
        {
            //if (!soInfo.BaseInfo.PayWhenReceived.Value && soInfo.BaseInfo.GainPoint != 0)
            //{
            //    //给款到发货的客户加积分
            //    ExternalDomainBroker.AdjustPoint(new BizEntity.Customer.AdjustPointRequest
            //    {
            //        CustomerSysNo = soInfo.BaseInfo.CustomerSysNo,
            //        Memo = string.Format("SO#:{0}-购物送积分", soInfo.SysNo),
            //        OperationType = ECCentral.BizEntity.Customer.AdjustPointOperationType.AddOrReduce,
            //        Point = soInfo.BaseInfo.GainPoint,
            //        PointType = (int)ECCentral.BizEntity.Customer.AdjustPointType.SalesDiscountPoint,
            //        SOSysNo = soInfo.SysNo,
            //        Source = SOConst.DomainName,
            //    });
            //}
        }

        private static string L(string key, params object[] args)
        {
            string multiLangText = ECommerce.WebFramework.LanguageHelper.GetText(key);
            return string.Format(multiLangText, args);
        }
    }
}
