/*********************************************************************************************
// Copyright (c) 2012, Newegg (Chengdu) Co., Ltd. All rights reserved.
// Created by Jin.J.Qin at 3/23/2012
// Target Framework : 4.0
// Class Name : ICustomerBizInteract
// Description : Customer Domain向其它Domain提供Biz服务的接口//
//*********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.IBizInteract
{
    public interface ICustomerBizInteract
    {
        #region 顾客信息相关
        /// <summary>
        /// 根据CustomerSysNo的列表返回Customer所有信息对象
        /// </summary>
        /// <param name="customerSysNoList"></param>
        /// <returns></returns>
        CustomerInfo GetCustomerInfo(int customerSysNo);

        /// <summary>
        /// 根据CustomerSysNo的返回Customer基本信息对象
        /// </summary>
        /// <param name="customerSysNo">Customer SysNo</param>
        /// <returns>Customer基本信息对象列表</returns>
        CustomerBasicInfo GetCustomerBasicInfo(int customerSysNo);

        /// <summary>
        /// 根据CustomerSysNo的列表返回Customer基本信息对象列表
        /// </summary>
        /// <param name="sysNoList">Customer SysNo List</param>
        /// <returns>Customer基本信息对象列表</returns>
        List<CustomerBasicInfo> GetCustomerBasicInfo(List<int> customerSysNoList);

        /// <summary>
        /// 获取系统账户，用来发放积分的账户
        /// </summary>
        /// <returns></returns>
        List<CustomerBasicInfo> GetSystemAccount(string webChannelID);

        /// <summary>
        /// 获取顾客等级
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        CustomerRank GetCustomerRank(int customerSysNo);

        /// <summary>
        /// 获取用户的可用积分数
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        int GetCustomerVaildScore(int customerSysNo);

        /// <summary>
        /// 设置客户是否为为恶意用户
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="isMalice">true-是恶意客户；false-不是恶意客户</param>
        /// <param name="log"></param>
        void SetMaliceCustomer(int customerSysNo, bool isMalice, string memo, int? SoSysNo);

        /// <summary>
        /// 根据订单金额更新客户累计购买金额 
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="orderedAmt"></param>
        void UpdateCustomerOrderedAmount(int customerSysNo, decimal orderedAmt);

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns>true:存在, false:不存在</returns>
        bool CustomerIsExists(int customerSysNo);

        //获取恶意用户列表
        /// <summary>
        /// 获取恶意用户列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>恶意用户列表</returns>
        List<CustomerInfo> GetMalevolenceCustomerList(string companyCode);

        #endregion

        #region CSTool

        /// <summary>
        /// 根据公司编码获取FPCheck列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>FPCheck列表</returns>
        List<FPCheck> GetFPCheckList(string companyCode);

        /// <summary>
        /// 根据公司编号获得所有的自动审单配置项
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<OrderCheckMaster> GetOrderCheckList(string companyCode);
        #endregion

        #region 账期相关

        /// <summary>
        /// 调整账期额度
        /// </summary>
        /// <param name="customerSysNo">顾客sysno</param>
        /// <param name="receivableAmount">在原来额度基础上调整的值</param>
        void AdjustCustomerCreditLimit(int customerSysNo, decimal adjustAmount);

        /// <summary>
        /// 将用户的账期额度直接设置为输入的值
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="limitAmount"></param>
        void SetCustomerCreditLimit(int customerSysNo, decimal limitAmount);
        /// <summary>
        /// 账期支付检查
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="amount"></param>
        void AdjustCreditLimitPreCheck(int customerSysNo, decimal amount); //IPP3:ISetCollectionPeriodAndRatingV31.AdjustCreditLimitPreCheck
        #endregion

        #region 积分

        /// <summary>
        /// 积分换算为钱的比例，如：该比例设置为10，意思是说 10个积分=1元
        /// </summary>
        /// <returns></returns>
        decimal GetPointToMoneyRatio();

        #region 积分相关 for so
        /// <summary>
        /// 调整积分的预检查
        /// </summary>
        /// <param name="info"></param>
        void AdjustPointPreCheck(ECCentral.BizEntity.Customer.AdjustPointRequest info); //IPP3: IAdjustPointV31.PreAdjustPoint
        /// <summary>
        /// 调整积分
        /// </summary>
        /// <param name="info">积分调整信息</param>
        void AdjustPoint(AdjustPointRequest info);
        /// <summary>
        /// 拆分主单积分消费记录
        /// </summary>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="master">主订单信息，请传送PointPay、CompanyCode、SysNo字段</param>
        /// <param name="subSoList">子订单信息，请传送PointPay、CompanyCode、SysNo字段</param>
        void SplitSOPointLog(SOBaseInfo master, List<SOBaseInfo> subSoList);
        /// <summary>
        /// 取消拆单 即 合并子单积分消费记录
        /// </summary>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="master">主订单信息，需要订单号和积分支付数量</param>
        /// <param name="subSoList">子订单信息，需要订单号和积分支付数量</param>
        void CancelSplitSOPointLog(SOBaseInfo master, List<SOBaseInfo> subSoList);
        /// <summary>
        /// 调整客户积分有效期
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="PointExpiringDate">积分有效期</param>
        /// <param name="memo">日志、备注信息</param>
        /// <param name="baseEntity">基础类对象，包含了InUser/EditUser/WebChannel/CompanyCode/StoreCompanyCode/LanguageCode等信息</param>
        void UpatePointExpiringDate(int customerSysNo, DateTime pointExpiringDate, string memo);
        #endregion

        #region 积分相关 for RMA

        /// <summary>
        /// 创建补偿积分申请单，并记录日志，返回补偿积分申请单编号
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="adjustAccount">调整积分的账户信息</param>
        /// <param name="point">需要调整的积分（正数-增加积分；负数-减少积分）</param>
        /// <param name="logType">日志类型</param>
        /// <param name="productSysNoList">商品系统编号列表</param>
        /// <param name="memo">备注</param>
        /// <returns>补偿积分申请单系统编号</returns>
        /// <param name="receivableAmount">基础类对象，包含了InUser/EditUser/WebChannel/CompanyCode/StoreCompanyCode/LanguageCode等信息</param>
        int CreateAdjustPointRequest(int customerSysNo, int soSysNo, string adjustAccount, int point, int logType, List<int> productSysNoList, string memo);
        /// <summary>
        ///
        /// </summary>
        /// <param name="requestSysNo">RMA申请单系统编号</param>
        /// <returns>状态</returns>
        int GetCustomerPointAddRequestStatus(int requestSysNo);
        /// <summary>
        ///
        /// </summary>
        /// <param name="requestSysNo">RMA申请单系统编号</param>
        void AbandonAdjustPointRequest(int requestSysNo);
        /// <summary>
        /// 根据SO编号和产品编号列表获取价保分数
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList);

        #endregion
        #endregion

        #region 经验值
        /// <summary>
        /// 调整客户经验值
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="ajustAmount">调整的值</param>
        /// <param name="eperienceAdjustType">经验值调整原来</param>
        /// <param name="memo"></param>
        void AdjustCustomerExperience(int customerSysNo, decimal ajustAmount, ExperienceLogType eperienceAdjustType, string memo);
        #endregion

        #region 余额

        /// <summary>
        /// 检查用户余额是否足够
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="paymentAmount">将要使用的金额</param>
        /// <returns>true-余额足够；false-余额不足</returns>
        bool CheckCustomerRemainingAmount(int customerSysNo, decimal paymentAmount);

        /// <summary>
        /// 调整余额前的检查
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        void AdjustPrePayPreCheck(CustomerPrepayLog info); //IPP3: IBalanceAccountV31.BalanceAccountPreCheck

        /// <summary>
        /// 调整余额
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <param name="note"></param>
        void AdjustPrePay(CustomerPrepayLog adjustInfo);
        #endregion

        #region 地址相关
        /// <summary>
        /// 更新客户送货地址　　
        /// </summary>
        /// <param name="shippingInfo">送货信息对象</param>
        void UpdateCustomerShippingInfo(ShippingAddressInfo shippingInfo);
        #endregion

        #region 权限

        /// <summary>
        /// 获取客户权限列表
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <returns>客户权限列表</returns>
        List<CustomerRight> GetCustomerRight(int customerSysNo);

        #endregion

        #region 增值税

        /// <summary>
        /// 设置客户订单增值税　
        /// </summary>
        /// <param name="VATSysNo">增值税发票系统编号</param>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="bankAcct">银行账号</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="companyAddress">公司地址</param>
        /// <param name="companyPhone">公司电话</param>
        /// <param name="taxAccount">税号</param>
        /// <param name="isDefault">是否存在认证证书</param>
        /// <param name="IsUpdate">是更新还是创建</param>
        void SetCustomerValueAddedTax(int VATSysNo, int customerSysNo, string bankAcct, string companyName, string companyAddress, string companyPhone, string taxAccount, bool isDefault, bool IsUpdate);

        #endregion

        #region 奖品
        /// <summary>
        /// 领取奖品
        /// </summary>
        /// <param name="customerSysNo">用户SysNO</param>
        /// <param name="productSysNo">奖品系统编号</param>
        /// <param name="soSysNo">传入订单号，以记录该奖品由哪个订单领走</param>
        void GetGift(int customerSysNo, int productSysNo, int soSysNo);

        /// <summary>
        /// 返还订单的奖品 订单作废时需返还奖品
        /// IPP3参考: IMaintainGiftStatusV31.VoidGiftForSO()
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="soSysNo">订单编号</param>
        void ReturnGiftForSO( int soSysNo);  //IPP3: IMaintainGiftStatusV31.VoidGiftForSO()
        #endregion

        #region 其它业务
        /// <summary>
        /// 取得短信内容
        /// </summary>
        /// <param name="webChannelID">渠道ID</param>
        /// <param name="languageCode">语言Code</param>
        /// <param name="shipTypeSysNo">运输方式系统编号</param>
        /// <param name="Type">短信类型</param>
        /// <returns></returns>
        string GetSMSContent(string webChannelID, string languageCode, int shipTypeSysNo, SMSType Type);

        /// <summary>
        /// 关闭顾客来电
        /// </summary>
        /// <param name="ReferenceType">引用方的类型，有RMA 和投诉 </param>
        /// <param name="ReferenceSysNo">对应的系统编号</param>
        /// <param name="note">关闭理由</param>
        void CloseCallsEvents(CallingReferenceType referenceType, int referenceSysNo, string note);

        #endregion

        #region 补偿退款单相关
        /// <summary>
        /// 修改补偿退款单状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Status"></param>
        /// <param name="RefundUserSysNo"></param>
        void AuditRefundAdjust(int SysNo, RefundAdjustStatus Status, int? RefundUserSysNo,DateTime? AuditTime);

        /// <summary>
        /// 获取补偿退款单的状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        RefundAdjustStatus? GetRefundAdjustStatus(int SysNo);
        #endregion

        #region 发短信
        string SendSMS(string cellPhone, string message);
        #endregion

    }
}