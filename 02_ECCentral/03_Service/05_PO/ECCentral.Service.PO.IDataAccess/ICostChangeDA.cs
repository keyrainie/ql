using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface ICostChangeDA
    {
        /// <summary>
        /// 加载成本变价单基础信息
        /// </summary>
        /// <param name="consignSettlementID"></param>
        /// <returns></returns>
        CostChangeBasicInfo LoadCostChangeBasicInfo(int ccSysNo);

        /// <summary>
        /// 加载成本变价单详细信息
        /// </summary>
        /// <param name="consignSettlementID"></param>
        /// <returns></returns>
        List<CostChangeItemsInfo> LoadCostChangeItemList(int ccSysNo);

        /// <summary>
        /// 编辑成本变价单时删除明细
        /// </summary>
        /// <param name="deleteItemInfo"></param>
        void DeleteCostChangeItems(CostChangeItemsInfo deleteItemInfo);

        /// <summary>
        /// 编辑成本变价单
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        CostChangeInfo UpdateCostChange(CostChangeInfo costChangeInfo);
        /// <summary>
        /// 创建成本变价单
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        CostChangeInfo CreateCostChange(CostChangeInfo costChangeInfo);
        /// <summary>
        /// 修改成本变价单状态
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        CostChangeInfo UpdateCostChangeStatus(CostChangeInfo costChangeInfo);
        /// <summary>
        /// 审核时修改成本变价单状态
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        CostChangeInfo UpdateCostChangeAuditStatus(CostChangeInfo costChangeInfo);
    }
}
