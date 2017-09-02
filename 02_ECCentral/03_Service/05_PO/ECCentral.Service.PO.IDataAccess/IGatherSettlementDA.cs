using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IGatherSettlementDA
    {
        /// <summary>
        /// 获取代收结算单Items(Invoice)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<GatherSettlementItemInfo> LoadConsignSettlementAllShippingCharge(GatherSettlementInfo info);

        List<GatherSettlementItemInfo> QueryConsignSettlementProductList(GatherSettleItemsQueryFilter queryFilter, out int totalCount);

        List<GatherSettlementItemInfo> QueryConsignSettlementProductList(GatherSettlementInfo info);

        List<GatherSettlementItemInfo> QueryConsignSettleGatherROAdjust(GatherSettlementInfo info);

        /// <summary>
        /// 加载代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementInfo LoadGatherSettlement(GatherSettlementInfo info);

        /// <summary>
        /// 创建新的代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementInfo CreateGatherSettlement(GatherSettlementInfo info);

        /// <summary>
        ///  更新代收结算单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementInfo UpdateGatherSettlement(GatherSettlementInfo info);

        /// <summary>
        /// 更新代收结算单状态(是否更新审核审核时间)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementInfo UpdateGatherSettlementStatus(GatherSettlementInfo info, bool needUpdateTime);

        /// <summary>
        /// 更新代收结算单的结算状态
        /// </summary>
        /// <param name="info"></param>
        /// <param name="needUpdateTime"></param>
        /// <returns></returns>
        GatherSettlementInfo UpdateGatherSettlementSettleStatus(GatherSettlementInfo info);

        /// <summary>
        /// 获取代收结算单相关删除商品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<GatherSettlementItemInfo> GetSettleGatherItems(GatherSettlementInfo info);

        List<GatherSettlementItemInfo> GetSettleGatherItems(int settleSysNo);

        List<GatherSettlementItemInfo> GetSettleGatherItemsInfoPage(GatherSettlementInfo info);

        /// <summary>
        /// 获取代收结算单基本信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementInfo GetVendorSettleGatherInfo(GatherSettlementInfo info);

        /// <summary>
        ///  删除代收结算单Item
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        GatherSettlementItemInfo DeleteSettleItem(GatherSettlementItemInfo info, int settleSysNo);

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetGatherSettlementSysNoListByVendorSysNo(int vendorSysNo);

        List<SettleItemInfo> GetSettleItemList(int settleSysNo);
    }
}
