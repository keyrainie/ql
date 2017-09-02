using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Invoice.IDataAccess
{
    /// <summary>
    /// NetPay数据库持久化接口
    /// </summary>
    public interface INetPayDA
    {
        /// <summary>
        /// 创建netpay
        /// </summary>
        /// <param name="entity">netpay实体信息</param>
        /// <returns>创建好的netpay</returns>
        NetPayInfo Create(NetPayInfo entity);

        /// <summary>
        /// 审核NetPay
        /// </summary>
        /// <param name="sysNo">Netpay系统编号</param>
        void UpdateApproveInfo(int sysNo, NetPayStatus status);

        /// <summary>
        /// 修改netpay状态
        /// </summary>
        /// <param name="sysNo">NetPay系统编号</param>
        /// <param name="status">目标状态</param>
        void UpdateStatus(int sysNo, NetPayStatus status);

        /// <summary>
        /// 根据订单系统编号作废NetPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void AbandonBySOSysNo(int soSysNo);

        /// <summary>
        /// 审查NetPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void UpdateReviewInfo(int soSysNo);

        /// <summary>
        /// 根据netpay系统编号加载netpay
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        NetPayInfo LoadBySysNo(int sysNo);

        /// <summary>
        /// 根据查询条件获取netpay列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<NetPayInfo> GetListByCriteria(NetPayInfo query);

        /// <summary>
        /// 根据关联的订单系统编号获取netpay列表
        /// </summary>
        /// <param name="relatedSoSysNo">关联订单系统编号</param>
        /// <returns></returns>
        List<NetPayInfo> GetListByRelatedSoSysNo(int relatedSoSysNo);

        /// <summary>
        /// 获取外部引用key
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        string GetExternalKeyBySOSysNo(int soSysNo);

        /// <summary>
        /// 更新netpay金额
        /// </summary>
        /// <param name="soInfo"></param>
        void UpdateMasterSOAmt(SOBaseInfo soInfo);

        /// <summary>
        /// 根据订单编号取得订单有效的netpay
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        NetPayInfo GetValidBySOSysNo(int soSysNo);

        /// <summary>
        /// 获取最后一笔作废的netpay记录
        /// </summary>
        /// <param name="relatedSoSysNo">关联订单系统编号</param>
        /// <returns></returns>
        NetPayInfo GetLastAboundedByRelatedSoSysNo(int relatedSoSysNo);

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        bool IsExistOriginalBySOSysNo(int soSysNo);

        #region [For SO Domain]

        /// <summary>
        /// 拆分NetPay时更新NetPay状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatusSplitForSO(NetPayInfo entity);

        /// <summary>
        /// 作废拆分NetPay
        /// </summary>
        /// <param name="master">主单信息</param>
        /// <param name="subList">子单列表</param>
        /// <param name="externalKey">externalKey</param>
        void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList, string externalKey);

        #endregion [For SO Domain]
    }
}