using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Income;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface ISOIncomeDA
    {
        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SOIncomeInfo Create(SOIncomeInfo entity);

        /// <summary>
        /// 加载销售收款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SOIncomeInfo LoadBySysNo(int sysNo);

        /// <summary>
        /// 更新收款单状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatus(SOIncomeInfo entity);

        /// <summary>
        /// 更新收款单状态为已处理，用于收款单自动确认时更新母单的收款状态为已处理
        /// </summary>
        /// <param name="sysNoList">收款单列表</param>
        void UpdateToProcessedStatus(List<SOIncomeInfo> sysNoList);

        /// <summary>
        /// 更新主单收款单金额
        /// </summary>
        /// <param name="soInfo"></param>
        void UpdateMasterSOAmt(SOBaseInfo soBaseInfo);

        /// <summary>
        /// 根据查询条件取得收款单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<SOIncomeInfo> GetListByCriteria(int? sysNo, int? orderSysNo, SOIncomeOrderType? orderType,List<SOIncomeStatus> soIncomeStatus);

        /// <summary>
        /// 根据订单系统编号列表取得收款单列表
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <returns></returns>
        List<SOIncomeInfo> GetListBySOSysNoList(List<int> soSysNoList);

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        SOIncomeInfo GetValid(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 根据单据类型和单据编号取得已经确认的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        SOIncomeInfo GetConfirmed(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="referenceID">凭证号</param>
        void SetReferenceID(int sysNo, string referenceID);

        /// <summary>
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="incomeAmt">实收金额</param>
        void SetIncomeAmount(int sysNo, decimal incomeAmt);

        #region [For SO Domain]

        /// <summary>
        /// 为PendingList生成销售收款单时需要调用，用来更新收款单单据金额
        /// </summary>
        /// <param name="soIncomeSysNo">销售-收款单系统编号</param>
        /// <param name="orderAmt">单据金额</param>
        void UpdateOrderAmtForSO(int sysNo, decimal orderAmt);

        /// <summary>
        /// 拆分订单时更新收款单信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatusSplitForSO(SOIncomeInfo entity);

        void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList);

        #endregion [For SO Domain]

        int GetROSOSysNO(int orderSysNo, int orderType);

        int GetSOIncomeBankInfoRefundPayType(int sosysNo);

        int GetRMAReundRefundPayType(int sosysNo);

        int GetSOIncomeBankInfoStatus(int sosysNo);

        decimal GetSOIncomeAmt(int sosysNo);

        /// <summary>
        /// 通过订单号，获取订单的Pos信息（预付款、银行卡付款、现金付款）
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <returns></returns>
        PosInfo GetPosInfoByOrderSysNo(int orderSysNo);

        SOFreightStatDetail LoadSOFreightConfirmBySysNo(int sysNo);

        void SOFreightConfirm(SOFreightStatDetail detail);

        void RealFreightConfirm(SOFreightStatDetail detail);
        /// <summary>
        /// 获取所有需要对账的关务对接相关信息
        /// </summary>
        /// <returns></returns>
        List<VendorCustomsInfo> QueryVendorCustomsInfo();

        /// <summary>
        /// 获取退款单编号
        /// </summary>
        /// <returns></returns>
        List<int> GetSysNoListByRefund();
    }
}