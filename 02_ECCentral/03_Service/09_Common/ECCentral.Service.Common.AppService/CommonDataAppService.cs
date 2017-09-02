using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(CommonDataAppService))]
    public class CommonDataAppService
    {
        public virtual List<WebChannel> GetWebChannelList(string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetWebChannelList(companyCode);
        }

        public virtual List<WebChannel> GetWebChannelsByUser(string companyCode, int userSysNo)
        {
            List<WebChannel> list = ObjectFactory<CommonDataProcessor>.Instance.GetWebChannelListByUser(companyCode, userSysNo);

            return list;
        }

        public virtual List<Company> GetCompanyList()
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetCompanyList();
        }

        public List<Company> GetCompaniesByUser(int userSysNo)
        {
            List<Company> list = ObjectFactory<CommonDataProcessor>.Instance.GetCompanyListByUser(userSysNo);

            return list;
        }

        public virtual List<ShippingType> GetShippingTypeList(string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetShippingTypeList(companyCode);
        }

        public virtual ShippingType GetShippingTypeBySysNo(int shipTypeSysNo)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetShippingTypeBySysNo(shipTypeSysNo);
        }

        public virtual List<PayType> GetPayTypeList(string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetPayTypeList(companyCode);
        }

        public virtual PayType GetPayTypeBySysNo(int payTypeSysNo)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetPayType(payTypeSysNo);
        }
        
        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        public virtual PayType GetPayTypeByID(string payTypeID)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetPayTypeByID(payTypeID);
        }

        public virtual void SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            ObjectFactory<CommonDataProcessor>.Instance.SendSMS(phoneNumber, content, priority);
        }

        public virtual int GetUserSysNo(string userAccount)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetUserSysNo(userAccount) ?? 0;
        }

        public virtual List<StockInfo> GetStockList(string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetStockList(companyCode);
        }

        public virtual string GetSystemConfigurationValue(string key, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetSysConfigurationValue(key, companyCode);
        }

        /// <summary>
        /// 根据key添加或者更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">Value</param>
        /// <param name="updateUserSysNo">操作用户编号</param>
        /// <param name="companyCode">公司编号</param>
        public virtual bool UpdateSystemConfigurationValueByKey(string key, string value, int updateUserSysNo, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.UpdateSystemConfigurationValueByKey(key, value, updateUserSysNo, companyCode);
        }

        public virtual string GetUserFullName(string userID, bool isSysNo)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetUserFullName(userID, isSysNo);
        }

        public virtual List<DepartmentInfo> GetAllEffectiveDepartment(string companyCode, string languageCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetAllEffectiveDepartment(companyCode, languageCode);
        }

        public virtual List<CurrencyInfo> GetCurrencyList()
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetCurrencyList();
        }

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public virtual List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetSpecialItemShipRule(shipTypeSysNo, companyCode);
        }

        /// <summary>
        /// 获取商品配送规则
        /// </summary>
        /// <param name="c3SysNoStr">商品3级分类编号</param>
        /// <param name="productSysNoStr">商品编号序列</param>
        /// <param name="provinceSysNo">省编号</param>
        /// <param name="citySysNo">市编号</param>
        /// <param name="areaSysno">区编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public List<ItemShipRuleInfo> GetItemShipRuleList(string c3SysNoStr, string productSysNoStr, int? provinceSysNo, int? citySysNo, int? areaSysno, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetItemShipRuleList(c3SysNoStr, productSysNoStr, provinceSysNo, citySysNo, areaSysno, companyCode);
        }

        public virtual decimal GetExchangeRateByCurrencySysNo(int sysNo, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetExchangeRateByCurrencySysNo(sysNo, companyCode);
        }

        public virtual List<UserInfo> GetCustomerServiceList(string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetCustomerServiceList(companyCode);
        }

        public virtual int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetUserSysNo(loginName, sourceDirectoryKey, companyCode);
        }

        public UserInfo GetUserInfoBySysNo(int userSysNo)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetUserInfoBySysNo(userSysNo);
        }

        public List<Language> GetAllLanguageList()
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetAllLanguageList();
        }
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CreateApplicationEventLog(ApplicationEventLog entity)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.CreateApplicationEventLog(entity);
        }
    }
}