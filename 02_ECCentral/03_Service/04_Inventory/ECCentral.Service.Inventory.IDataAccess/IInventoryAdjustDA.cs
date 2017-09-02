using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IInventoryAdjustDA
    {

        #region 商品批号相关

        /// <summary>
        /// 根据单据编号 获取批号信息表信息
        /// </summary>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        List<InventoryBatchDetailsInfo> GetBatchDetailsInfoEntityListByNumber(int DocumentNumber);

        /// <summary>
        /// 给仓库发消息，调用SP调整批次信息
        /// </summary>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        int AdjustBatchNumberInventory(string paramXml);

        void UpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType, string action);

        void DeleteBatchItemOfSTB(int productSysNo, int documentNumber);

        void SourceUpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType);

        void TargetUpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType);

        void DeleteAllBatchItem(int documentNumber, string deleteType);

        #endregion 商品批号相关
    }
}
