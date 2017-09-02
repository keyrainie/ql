using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPayItemDA
    {
        /// <summary>
        /// 创建付款单
        /// *需要同时写付款单扩展信息表Finance_Pay_Ex，建立应付款和付款单的关联关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PayItemInfo Create(PayItemInfo entity);

        /// <summary>
        /// 建立应付款和付款单的关联
        /// </summary>
        /// <param name="input"></param>
        void CreatePayEx(PayItemInfo entity);

        /// <summary>
        /// 更新付款单
        /// </summary>
        /// <param name="entity"></param>
        PayItemInfo Update(PayItemInfo entity);

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="entity"></param>
        PayItemInfo SetReferenceID(int sysNo, string referenceID);

        /// <summary>
        /// 对于一个PO单对应的付款状态为FullPay的应付款，如果这个PO单对应的应付款有存在付款状态为Origin的付款单，
        /// 系统自动将这些付款单的付款状态从Origin置为Abandon
        /// </summary>
        /// <param name="entity"></param>
        void SetAbandonOfFullPay(PayItemInfo entity);

        /// <summary>
        /// 更新付款单状态
        /// </summary>
        /// <param name="entity"></param>
        PayItemInfo UpdateStatus(PayItemInfo entity);

        /// <summary>
        /// 更新付款单状态和编辑人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PayItemInfo UpdateStatusAndEditUser(PayItemInfo entity);

        /// <summary>
        /// 根据付款单编号加载付款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        PayItemInfo Load(int sysNo);

        /// <summary>
        /// 根据查询条件加载付款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<PayItemInfo> GetListByCriteria(PayItemInfo query);

        /// <summary>
        /// 取得系统用户编号
        /// SELECT TOP 1 SysNo
        ///		FROM IPP3.dbo.Sys_User WITH(NOLOCK)
        ///		WHERE username='System'
        /// </summary>
        /// <returns></returns>
        int GetSystemUserSysNo();

        /// <summary>
        /// 是否是最后一个未作废付款单
        /// </summary>
        /// <param name="payItemInfo"></param>
        /// <returns></returns>
        bool IsLastUnAbandon(PayItemInfo payItemInfo);

        /// <summary>
        /// 根据付款单状态取得满足条件的付款单列表
        /// 只取PO单、代销结算单和代收结算单的数据
        /// </summary>
        /// <param name="status">付款单状态</param>
        List<PayItemInfo> GetListByStatus(PayItemStatus status);

        #region [For PO Domain]

        /// <summary>
        /// 更新付款单可用金额
        /// </summary>
        /// <param name="orderType">单据类型</param>
        /// <param name="orderSysNo">单据编号</param>
        /// <param name="availableAmt">可用金额</param>
        void UpdateAvailableAmt(PayableOrderType orderType, int orderSysNo, decimal availableAmt);

        /// <summary>
        /// 付款单是否是预付款
        /// </summary>
        /// <param name="orderType">单据类型</param>
        /// <param name="orderSysNo">单据编号</param>
        /// <returns></returns>
        bool IsAdvanced(PayableOrderType orderType, int orderSysNo);

        /// <summary>
        /// 锁定或取消锁定付款单
        /// </summary>
        /// <param name="payItemSysNoList">要操作的付款单系统编号列表</param>
        /// <param name="status">要更新付款单的状态：Lock or Origin</param>
        /// <returns>更新成功的付款单</returns>
        List<PayItemInfo> LockOrUnLockBySysNoList(List<int> payItemSysNoList, PayItemStatus status);

        void InsertPayItemInfo(PayItemInfo info);

        PayItemInfo GetPayItemInfoByPOSysNo(int poSysNo);

        #endregion [For PO Domain]
    }
}