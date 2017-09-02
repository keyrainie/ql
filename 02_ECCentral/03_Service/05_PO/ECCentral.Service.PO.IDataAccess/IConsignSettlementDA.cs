using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IConsignSettlementDA
    {
        /// <summary>
        /// 加载代销结算单详细信息
        /// </summary>
        /// <param name="consignSettlementID"></param>
        /// <returns></returns>
        ConsignSettlementInfo LoadConsignSettlementInfo(int consignSettlementSysNo);

        /// <summary>
        /// 加载代销结算单Item信息
        /// </summary>
        /// <param name="consignSettlementSysNo"></param>
        /// <returns></returns>
        List<ConsignSettlementItemInfo> LoadConsignSettlementItemList(int consignSettlementSysNo);

        /// <summary>
        /// 加载代销结算单 - 代销转财务记录
        /// </summary>
        /// <param name="consignToAccountLogSysNo"></param>
        /// <returns></returns>
        ConsignToAcctLogInfo LoadConsignToAccountLogInfo(int? consignToAccountLogSysNo);

        /// <summary>
        /// 代销转财务记录是否在其他供应商的结算单中存在
        /// </summary>
        /// <param name="accountLogSysNo"></param>
        /// <returns></returns>
        bool IsAccountLogExistOtherVendorSettle(int accountLogSysNo);
        /// <summary>
        /// 更新代销结算单信息
        /// </summary>
        /// <param name="settlementInfo"></param>
        /// <returns></returns>
        ConsignSettlementInfo UpdateConsignSettlementInfo(ConsignSettlementInfo settlementInfo);

        /// <summary>
        /// 删除单个代销结算单商品
        /// </summary>
        ConsignSettlementItemInfo DeleteConsignSettlementItemInfo(ConsignSettlementItemInfo settlementItemInfo);

        /// <summary>
        /// 更新代销转财务记录状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="newStatus"></param>
        void UpdateConsignToAccountLogStatus(int sysNo, ConsignToAccountLogStatus newStatus);

        /// <summary>
        /// 更新代销结算单状态(审核)
        /// </summary>
        /// <param name="settlementInfo"></param>
        /// <returns></returns>
        ConsignSettlementInfo UpdateAuditStatus(ConsignSettlementInfo settlementInfo);
        /// <summary>
        /// 更新代销结算单状态(结算)
        /// </summary>
        /// <param name="settlementInfo"></param>
        /// <returns></returns>
        ConsignSettlementInfo UpdateSettleStatus(ConsignSettlementInfo settlementInfo);

        /// <summary>
        /// 创建代销结算单信息
        /// </summary>
        /// <param name="consignSettlementInfo"></param>
        /// <returns></returns>
        ConsignSettlementInfo CreateConsignSettlementInfo(ConsignSettlementInfo consignSettlementInfo);

        /// <summary>
        ///  创建代销结算单 Item 信息
        /// </summary>
        /// <param name="settlementItemInfo"></param>
        /// <returns></returns>
        ConsignSettlementItemInfo CreateConsignSettlemtnItemInfo(ConsignSettlementItemInfo settlementItemInfo);

        /// <summary>
        /// 创建代销转财务记录
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        ConsignToAcctLogInfo CreateConsignToAccountLog(ConsignToAcctLogInfo logInfo);

        /// <summary>
        /// 创建代销转财务记录(Inventory)
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        ConsignToAcctLogInfo CreatePOConsignToAccLogForInventory(ConsignToAcctLogInfo logInfo);
        
        /// <summary>
        /// 结算 - 更改财务记录为最终结算
        /// </summary>
        void SettleConsignToAccountLog(int accountLogSysNo, decimal? cost, decimal foldCost);

        /// <summary>
        /// 取消结算 - 代销转财务记录
        /// </summary>
        /// <param name="accountLogSysNo"></param>
        void CancelSettleConsignToAccountLog(int accountLogSysNo);

        /// <summary>
        /// 获取PM Backup List
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        List<int> GetBackUpPMList(int userSysNo, string companyCode);

        /// <summary>
        /// 检查代销单归属PM（或归属PM的备份PM）与商品PM不一致
        /// </summary>
        /// <param name="pms"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool ExistsDifferentPMSysNo(List<int> pms, List<int> productSysNo, string companyCode);

        int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo);

        /// <summary>
        /// 根据供应商系统编号取得代销结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList);

        /// <summary>
        /// 获取统计后的规则对应的商品数量列表
        /// </summary>
        /// <param name="settleSysNo">结算单编号</param>
        /// <returns>获取统计后的规则对应的商品数量列表</returns>
        List<ConsignSettlementRulesInfo> GetSettleRuleQuantityCount(int settleSysNo);

        /// <summary>
        /// 修改规则状态
        /// </summary>
        /// <param name="settleRuleId">规则Id</param>
        /// <param name="status">状态</param>
        /// <param name="settledQuantity">已结算数量</param>
        /// <returns>成功返回真，否则返回假</returns>
        bool UpdateConsignSettleRuleStatusAndQuantity(int settleRuleId, ConsignSettleRuleStatus status, int settledQuantity);

          /// <summary>
        /// 获取存在验证规则
        /// </summary>
        /// <param name="settleRuleId">规则Id</param>
        void UpdateExistsConsignSettleRuleItemStatus(int settleRuleId);


        #region 经销商品结算

        /// <summary>
        /// 添加经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
         SettleInfo SettleInfoAdd(SettleInfo SettleInfo);

        /// <summary>
        /// 添加经销商品结算单子项
        /// </summary>
        /// <param name="SettleItemInfo"></param>
        /// <returns></returns>
         SettleItemInfo SettleItemInfoAdd(SettleItemInfo SettleItemInfo);

        /// <summary>
        /// 获取经销商品结算单信息
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
         SettleInfo GetSettleInfo(SettleInfo SettleInfo);
        /// <summary>
        /// 获取经销商品结算单子项
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
         List<SettleItemInfo> GetSettleItemInfo(SettleInfo SettleInfo);

        /// <summary>
        /// 获取经销商品结算单子项（附带着税金价款信息）
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
         List<SettleItemInfo> GetSettleItemInfoWithTaxAndCost(SettleInfo SettleInfo);

        /// <summary>
        /// 审核经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
         void AuditSettleAccount(SettleInfo SettleInfo);

        /// <summary>
        /// 修改Finance_Pay 支付状态（创建经销商结算单是 修改状态为2 ，如果作废做返回状态到 0）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="OrderSysNo"></param>
         void ChangeFinancePayStatus(PayableStatus status, int financePaySysNo);



         /// <summary>
         /// 修改支付字表的支付状态为已支付
         /// </summary>
         /// <param name="payItemStatus"></param>
         /// <param name="dtime"></param>
         /// <param name="financePaySysNo"></param>
         void FinancePayItemPaid(int financePaySysNo);

         /// <summary>
         /// 修改支付字表的支付状态为待支付状态
         /// </summary>
         /// <param name="payItemStatus"></param>
         /// <param name="financePaySysNo"></param>
         void FinancePayItemOrigin(int financePaySysNo);

        #endregion 
    }
}
