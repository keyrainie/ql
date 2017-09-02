using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPostPayDA
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PostPayInfo Create(PostPayInfo entity);

        List<PostPayInfo> GetListByConfirmedSOSysNo(int confirmedSOSysNo);

        List<PostPayInfo> GetListBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据订单系统编号作废PostPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void AbandonBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据订单系统编号和PostPay状态取得PostPay列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="status">PostPay状态，可以包含多种状态</param>
        /// <returns></returns>
        List<PostPayInfo> GetListBySOSysNoAndStatus(int soSysNo, params PostPayStatus[] status);

        /// <summary>
        /// 根据订单编号取得订单有效的postpay
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        PostPayInfo GetValidPostPayBySOSysNo(int soSysNo);

        /// <summary>
        /// 取得已确认的多付金额
        /// </summary>
        /// <param name="confirmedSOSysNo">订单系统编号，多个编号之间用逗号（,）隔开</param>
        /// <returns></returns>
        decimal GetRefundAmtByConfirmedSOSysNoList(string confirmedSOSysNo);

        #region [For SO Domain]

        /// <summary>
        /// 拆分PostPay时更新PostPay状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatusSplitForSO(PostPayInfo entity);

        /// <summary>
        /// 作废拆分PostPay
        /// </summary>
        /// <param name="master">主单信息</param>
        /// <param name="subList">子单列表</param>
        void AbandonSplitForSO(ECCentral.BizEntity.SO.SOBaseInfo master, List<ECCentral.BizEntity.SO.SOBaseInfo> subList);

        #endregion [For SO Domain]
    }
}