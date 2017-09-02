using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(CommonDataProcessor))]
    public class CommonDataProcessor
    {
        public virtual List<WebChannel> GetWebChannelList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetWebChannelList(companyCode);
        }

        public virtual WebChannel GetWebChannelByChannelID(string chanelID)
        {
            return ObjectFactory<ICommonDA>.Instance.GetWebChannelByChannelID(chanelID);
        }

        public virtual List<WebChannel> GetWebChannelListByUser(string companyCode, int userSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetWebChannelListByUser(companyCode, userSysNo);
        }

        public virtual List<Company> GetCompanyList()
        {
            return ObjectFactory<ICommonDA>.Instance.GetCompanyList();
        }

        public List<Company> GetCompanyListByUser(int userSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetCompanyListByUser(userSysNo);
        }

        public virtual List<ShippingType> GetShippingTypeList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetShippingTypeList(companyCode);
        }

        public virtual ShippingType GetShippingTypeBySysNo(int shipTypeSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetShippingTypeBySysNo(shipTypeSysNo);
        }

        public virtual List<PayType> GetPayTypeList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetPayTypeList(companyCode);
        }

        public virtual PayType GetPayType(int payTypeSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetPayTypeBySysNo(payTypeSysNo);
        }
        
        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        public virtual PayType GetPayTypeByID(string payTypeID)
        {
            return ObjectFactory<ICommonDA>.Instance.GetPayTypeByID(payTypeID);
        }

        public virtual void SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            ObjectFactory<ICommonDA>.Instance.SendSMS(phoneNumber, content, priority);
        }

        public virtual int? GetUserSysNo(string userAccount)
        {
            return ObjectFactory<ICommonDA>.Instance.GetUserSysNo(userAccount);
        }

        public virtual List<StockInfo> GetStockList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetStockList(companyCode);
        }

        public virtual string GetSysConfigurationValue(string key, string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetSysConfigurationValue(key, companyCode);
        }

        public virtual string GetUserFullName(string userID, bool isSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetUserFullName(userID, isSysNo);
        }

        /// <summary>
        /// 获取所有有效部门清单
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public virtual List<DepartmentInfo> GetAllEffectiveDepartment(string companyCode, string languageCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetDepartmentList(companyCode, languageCode)
                .Where(p => p.Status == DepartmentValidStatus.Active).ToList();
        }

        public virtual List<CurrencyInfo> GetCurrencyList()
        {
            return ObjectFactory<ICommonDA>.Instance.GetCurrencyList();
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
            return ObjectFactory<ICommonDA>.Instance.UpdateSystemConfigurationValueByKey(key,value,updateUserSysNo,companyCode);
        }

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public virtual List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetSpecialItemShipRule(shipTypeSysNo, companyCode);
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
            return ObjectFactory<ICommonDA>.Instance.GetItemShipRuleList(c3SysNoStr, productSysNoStr, provinceSysNo, citySysNo, areaSysno, companyCode);
        }

        /// <summary>
        /// 根据CurrencySysNO获取货币的ExchangeRate:
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual decimal GetExchangeRateByCurrencySysNo(int sysNo, string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetExchangeRateByCurrencySysNo(sysNo, companyCode);
        }

        public virtual List<UserInfo> GetCustomerServiceList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetCustomerServiceList(companyCode);
        }

        public UserInfo GetUserInfoBySysNo(int userSysNo)
        {
            return ObjectFactory<ICommonDA>.Instance.GetUserInfoBySysNo(userSysNo);
        }

        public virtual int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetUserSysNo(loginName, sourceDirectoryKey, companyCode);
        }

        public virtual List<Language> GetAllLanguageList()
        {
            return ObjectFactory<IMultiLanguageDA>.Instance.GetAllLanguageList();
        }
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CreateApplicationEventLog(ApplicationEventLog entity)
        {
            return ObjectFactory<ICommonDA>.Instance.CreateApplicationEventLog(entity);
        }
    }
}