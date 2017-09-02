using System;
using System.Collections.Generic;
using System.Linq;

using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ICommonBizInteract))]
    public class BizInteractAppService : ICommonBizInteract
    {
        public virtual WebChannel GetWebChannel(string channelId)
        {
            return ObjectFactory<CommonDataProcessor>.Instance.GetWebChannelByChannelID(channelId);
        }

        public virtual AreaInfo GetAreaInfo(int districtSysNo)
        {
            return ObjectFactory<AreaAppService>.Instance.Load(districtSysNo);
        }

        public virtual List<PayType> GetPayTypeList(string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetPayTypeList(companyCode);
        }

        public virtual PayType GetPayType(int payTypeSysNo)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetPayTypeBySysNo(payTypeSysNo);
        }
        
        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        public virtual PayType GetPayTypeByID(string payTypeID)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetPayTypeByID(payTypeID);
        }

        public virtual int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<LogAppService>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        public virtual void SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            ObjectFactory<CommonDataAppService>.Instance.SendSMS(phoneNumber, content, priority);
        }

        public virtual string GetSystemConfigurationValue(string key, string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetSystemConfigurationValue(key, companyCode);
        }

        public virtual string GetUserFullName(string userID, bool isSysNo)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserFullName(userID, isSysNo);
        }

        public virtual int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserSysNo(loginName, sourceDirectoryKey, companyCode);
        }
        public virtual int GetUserSysNo(string loginName)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserSysNo(loginName);
        }

        public string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ObjectFactory<ReasonCodeService>.Instance.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

        public UserInfo GetUserInfoBySysNo(int sysNo)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserInfoBySysNo(sysNo);
        }

        public string GetUniqueUserName(int userSysNo)
        {
            string uniqueUserName = string.Empty;
            UserInfo user = GetUserInfoBySysNo(userSysNo);
            if (user != null)
            {
                uniqueUserName = string.Format(@"{0}\{1}\{2}[{3}]", user.UserDisplayName, user.Domain, user.UserName, user.CompanyCode);  
            }
            return uniqueUserName;
        }

        /// <summary>
        /// 根据配送方式编号查找配送方式实体
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public ShippingType GetShippingType(int sysNo)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetShippingTypeBySysNo(sysNo);
        }

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetSpecialItemShipRule(shipTypeSysNo, companyCode);
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
            return ObjectFactory<CommonDataAppService>.Instance.GetItemShipRuleList(c3SysNoStr, productSysNoStr, provinceSysNo, citySysNo, areaSysno, companyCode);
        }

        /// <summary>
        /// 根据CurrencySysNO获取货币的ExchangeRate:
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public decimal GetExchangeRateBySysNo(int sysNo, string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetExchangeRateByCurrencySysNo(sysNo, companyCode);
        }

        #region ICommonBizInteract Members

        public CurrencyInfo GetCurrencyInfoBySysNo(int sysNo)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetCurrencyList().Where(x => x.SysNo == sysNo).SingleOrDefault();
        }

        #endregion ICommonBizInteract Members

        public void UpdateSystemConfigurationValue(string key, string value, string companyCode)
        {
        }

        /// <summary>
        /// 根据key添加或者更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">Value</param>
        /// <param name="updateUserSysNo">操作用户编号</param>
        /// <param name="companyCode">公司编号</param>
        public bool UpdateSystemConfigurationValueByKey(string key, string value, int updateUserSysNo, string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.UpdateSystemConfigurationValueByKey(key, value, updateUserSysNo, companyCode);
        }

        #region ICommonBizInteract Members

        public List<DateTime> GetHolidayList(string blockedService, string CompanyCode)
        {
            return ObjectFactory<HolidayProcessor>.Instance.GetHolidayList(blockedService, CompanyCode);
        }

        public List<Holiday> GetAllHolidaysAfterToday(string companyCode)
        {
            return ObjectFactory<HolidayProcessor>.Instance.GetHolidaysAfterToday(companyCode);
        }

        #endregion ICommonBizInteract Members

        public List<ShipTypeAreaUnInfo> QueryShipAreaUnByAreaSysNo(IEnumerable<int> shipTypeSysNoS, int areaSysNo)
        {
            return ObjectFactory<ShipTypeAreaUnProcessor>.Instance.QueryShipAreaUnByAreaSysNo(shipTypeSysNoS, areaSysNo); ;
        }
        public List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode)
        {
            return ObjectFactory<ShipTypeAreaUnProcessor>.Instance.GetShipTypeAreaUnList(companyCode);
        }

        public bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.CheckOperateRightForCurrentUser(productSysNo, pmSysNo);
        }

        public List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.GetProductLineSysNoByProductList(productSysNo);
        }

        public List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
        {
            return ObjectFactory<ProductLineProcessor>.Instance.GetProductLineInfoByPM(pmSysNo);
        }

        public List<Language> GetAllLanguageList()
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetAllLanguageList();
        }

        public void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            ObjectFactory<MultiLanguageProcessor>.Instance.SetMultiLanguageBizEntity(entity);
        }
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CreateApplicationEventLog(ApplicationEventLog entity)
        {
            return ObjectFactory<CommonDataAppService>.Instance.CreateApplicationEventLog(entity);
        }
    }
}
