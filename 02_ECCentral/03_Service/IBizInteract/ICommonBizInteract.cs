/*********************************************************************************************
// Copyright (c) 2012, Newegg (Chengdu) Co., Ltd. All rights reserved.
// Created by Jin.J.Qin at 3/23/2012
// Target Framework : 4.0
// Class Name : ICommonBizInteract
// Description : 向其它Domain提供基础的业务服务的接口//
//*********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.IBizInteract
{
    public interface ICommonBizInteract
    {
        /// <summary>
        /// 获取渠道信息
        /// </summary>
        /// <param name="channelId">渠道ID</param>
        /// <returns>渠道信息</returns>
        WebChannel GetWebChannel(string channelId);

        /// <summary>
        /// 获取地区详细信息
        /// </summary>
        /// <param name="districtSysNo">区(县)编号</param>
        /// <returns>地区详细信息</returns>
        AreaInfo GetAreaInfo(int districtSysNo);

        /// <summary>
        /// 获取所有的支付类型列表
        /// </summary>
        /// <returns></returns>
        List<PayType> GetPayTypeList(string companyCode);

        /// <summary>
        /// 根据支付类型编号取得支付类型信息
        /// </summary>
        /// <param name="payTypeSysNo">支付类型编号</param>
        /// <returns></returns>
        PayType GetPayType(int payTypeSysNo);
        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        PayType GetPayTypeByID(string payTypeID);

        /// <summary>
        /// 创建操作日志，提供统一的日志记录服务
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode);

        /// <summary>
        /// 短信发送接口
        /// </summary>
        /// <param name="phoneNumber">发送到的短信号码，多个号码可以用英文半角的逗号“,”或分号“;”隔开</param>
        /// <param name="content">短信内容</param>
        /// <param name="priority">发送的优先级</param>
        void SendSMS(string phoneNumber, string content, SMSPriority priority);

        /// <summary>
        /// 根据key获取IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string GetSystemConfigurationValue(string key, string companyCode);

        /// <summary>
        /// 根据key更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="companyCode"></param>
        void UpdateSystemConfigurationValue(string key, string value, string companyCode);


        /// <summary>
        /// 根据key添加或者更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">Value</param>
        /// <param name="updateUserSysNo">操作用户编号</param>
        /// <param name="companyCode">公司编号</param>
        bool UpdateSystemConfigurationValueByKey(string key, string value, int updateUserSysNo, string companyCode);

        /// <summary>
        /// 根据用户ID来获取用户显示名
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="isSysNo">指定第一个参数userID是否为User的SysNo，如果为false则说明为User的登录ID</param>
        /// <returns></returns>
        string GetUserFullName(string userID, bool isSysNo);

        /// <summary>
        /// 根据登录名称，域和companyCode获取用户编号
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="sourceDirectoryKey"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode);

        /// <summary>
        ///根据系统用户的编号获得系统用户信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        UserInfo GetUserInfoBySysNo(int sysNo);

        /// <summary>
        /// 获取唯一用户名称
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        string GetUniqueUserName(int userSysNo);

        /// <summary>
        ///  根据codeSysNo查询ReasonCode路径
        /// </summary>
        /// <param name="reasonCodeSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string GetReasonCodePath(int reasonCodeSysNo, string companyCode);

        /// <summary>
        /// 根据配送方式编号查找配送方式实体
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        ShippingType GetShippingType(int sysno);

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

        /// <summary>
        /// 根据CurrenyCode获取ExchangeRate
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        decimal GetExchangeRateBySysNo(int sysNo, string companyCode);

        /// <summary>
        /// 根据SYSNO获取货币信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CurrencyInfo GetCurrencyInfoBySysNo(int sysNo);

        /// <summary>
        /// 获取节假日
        /// </summary>
        /// <param name="blockedService"></param>
        /// <returns></returns>
        List<DateTime> GetHolidayList(string blockedService, string CompanyCode);

        /// <summary>
        /// 获取所有今天以及以后的节假日
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>今天以及以后的节假日列表</returns>
        List<Holiday> GetAllHolidaysAfterToday(string companyCode);

        /// <summary>
        /// IPP-Bob.H.Li获取指定配送方式指定地区的非表  Jack移植 2012-10-25
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式</param>
        /// <param name="areaSysNo">地区</param>
        /// <returns></returns>
        List<ShipTypeAreaUnInfo> QueryShipAreaUnByAreaSysNo(IEnumerable<int> shipTypeSysNoS, int areaSysNo);

        //获取非配送地区列表
        /// <summary>
        /// 获取非配送地区列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>非配送地区列表</returns>
        List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode);

        /// <summary>
        /// 根据传入的ProductSysno,PMSysNo 检测是否匹配
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo);

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        /// <param name="productLineSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo);

        /// <summary>
        /// 根据PM，获取其全部产品线
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo);

        /// <summary>
        /// 写入多语言
        /// </summary>
        /// <param name="entity"></param>
        void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity);

        /// <summary>
        /// 获取系统多言列表
        /// </summary>
        /// <returns></returns>
        List<Language> GetAllLanguageList();
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool CreateApplicationEventLog(ApplicationEventLog entity);
    }
}
