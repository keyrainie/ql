using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IShiftRequestItemBasketDA
    {
        /// <summary>
        /// 创建移仓篮Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int CreateShiftBasket(ShiftRequestItemInfo item);

        /// <summary>
        /// 检查Item的移出和移入仓库是否已经存在于当前移仓篮中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsExistSourceAndTargetStockInBasket(ShiftRequestItemInfo item);

        int GetNowShiftQtyGroupByProductSysNo(ShiftRequestItemInfo item);

        int GetStockAvailableQtyGroupByProductSysNo(ShiftRequestItemInfo item);

        /// <summary>
        /// 删除移仓篮Item 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int DeleteShiftBasket(ShiftRequestItemInfo item);

        /// <summary>
        /// 更新移仓篮Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateShiftBasket(ShiftRequestItemInfo item);
    }
}
