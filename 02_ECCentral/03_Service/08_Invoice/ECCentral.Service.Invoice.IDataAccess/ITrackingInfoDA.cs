using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface ITrackingInfoDA
    {
        /// <summary>
        /// 根据单据编号和单据类型取得跟踪信息
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        TrackingInfo LoadTrackingInfoByOrderSysNo(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 根据系统编号取得跟踪信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        TrackingInfo LoadTrackingInfoBySysNo(int sysNo);

        void UpdateTrackingInfoStatus(TrackingInfo entity);

        /// <summary>
        /// 跟新跟踪单
        /// </summary>
        /// <param name="entity"></param>
        void UpdateTrackingInfo(TrackingInfo entity);

        /// <summary>
        /// 是否已经存在跟踪单据
        /// </summary>
        /// <param name="orderSysNo">单据ID</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>存在则true；否则false</returns>
        bool ExistsTrackingInfo(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 创建跟踪单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TrackingInfo CreateTrackingInfo(TrackingInfo entity);

        /// <summary>
        /// 取得公司下所有的责任人
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<ResponsibleUserInfo> GetAllResponsibleUsers(string companyCode);

        /// <summary>
        /// 通过责任人姓名取得责任人邮件地址列表
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<string> GetEmailAddressByResponsibleUserName(string userName);

        /// <summary>
        /// 通过系统用户编号取得系统用户邮件地址
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        string GetEmailAddressByUserSysNo(int userSysNo);

        /// <summary>
        /// 取得收款单应收金额
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        decimal GetIncomeAmt(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 创建跟踪单负责人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponsibleUserInfo CreateResponsibleUser(ResponsibleUserInfo entity);

        /// <summary>
        /// 取得已经存在符合条件的跟踪单负责人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponsibleUserInfo GetExistedResponsibleUser(ResponsibleUserInfo entity);

        /// <summary>
        /// 更新跟踪单责任人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void UpdateResponsibleUser(ResponsibleUserInfo entity);

        /// <summary>
        /// 作废跟踪单责任人
        /// </summary>
        /// <param name="sysNo"></param>
        void AbandonResponsibleUser(int sysNo);

        /// <summary>
        /// 根据责任人编号加载责任人信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ResponsibleUserInfo LoadResponsibleUserBySysNo(int sysNo);

        List<TrackingInfo> GetNotClosedTrackingInfoBelongToCertainUser(string responsibleUserName);

        /// <summary>
        /// 修改跟踪单责任人姓名
        /// </summary>
        /// <param name="entity"></param>
        void UpdateTrackingInfoResponsibleUserName(TrackingInfo entity);

        /// <summary>
        /// 修改跟踪单损失类型
        /// </summary>
        /// <param name="entity"></param>
        void UpdateTrackingInfoLossType(TrackingInfo entity);
    }
}