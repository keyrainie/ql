using System.Collections.Generic;
using System.Data;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface ICommonDA
    {
        List<WebChannel> GetWebChannelList(string companyCode);

        WebChannel GetWebChannelByChannelID(string chanelID);

        List<WebChannel> GetWebChannelListByUser(string companyCode, int userSysNo);

        List<Company> GetCompanyList();

        List<Company> GetCompanyListByUser(int userSysNo);

        List<ShippingType> GetShippingTypeList(string companyCode);

        ShippingType GetShippingTypeBySysNo(int shipTypeSysNo);

        List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode);

        List<PayType> GetPayTypeList(string companyCode);

        PayType GetPayTypeBySysNo(int payTypeSysNo);

        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        PayType GetPayTypeByID(string payTypeID);

        int? GetUserSysNo(string userAccount);

        List<StockInfo> GetStockList(string companyCode);

        string GetSysConfigurationValue(string key, string companyCode);

        /// <summary>
        /// 根据key添加或者更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">Value</param>
        /// <param name="updateUserSysNo">操作用户编号</param>
        /// <param name="companyCode">公司编号</param>
        bool UpdateSystemConfigurationValueByKey(string key, string value, int updateUserSysNo, string companyCode);

        string GetUserFullName(string userID, bool isSysNo);

        List<UserInfo> GetUserInfoList(UserInfoQueryFilter filter);

        List<DepartmentInfo> GetDepartmentList(string companyCode, string languageCode);

        void SendSMS(string phoneNumber, string content, SMSPriority priority);

        List<UserInfo> GetBizOperationUser(ECCentral.QueryFilter.Common.BizOperationUserQueryFilter filter);

        List<CurrencyInfo> GetCurrencyList();

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode);

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
        List<ItemShipRuleInfo> GetItemShipRuleList(string c3SysNoStr, string productSysNoStr, int? provinceSysNo, int? citySysNo, int? areaSysno, string companyCode);

        decimal GetExchangeRateByCurrencySysNo(int sysNo, string companyCode);

        List<UserInfo> GetAllAuthSystemUser(string companyCode);

        /// <summary>
        /// 取得投递员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<UserInfo> GetFreightManList(string companyCode);

        List<UserInfo> GetCustomerServiceList(string companyCode);

        int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode);

        UserInfo GetUserInfoBySysNo(int userSysNo);
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool CreateApplicationEventLog(ApplicationEventLog entity);
    }
}