using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IShiftRequestDA
    {
        #region 移仓单主信息维护

        /// <summary>
        /// 根据SysNO获取移仓单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ShiftRequestInfo GetShiftRequestInfoBySysNo(int sysNo);

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ShiftRequestInfo GetProductLineInfo(int sysNo);

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ShiftRequestInfo CreateShiftRequest(ShiftRequestInfo entity);

        /// <summary>
        /// 更新移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ShiftRequestInfo UpdateShiftRequest(ShiftRequestInfo entity);

        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ShiftRequestInfo UpdateShiftRequestStatus(ShiftRequestInfo entity);

        /// <summary>
        /// 设置特殊状态
        /// </summary>
        /// <param name="entity"></param>
        ShiftRequestInfo UpdateSpecialShiftType(ShiftRequestInfo entity);
        /// <summary>
        /// 生成自增序列
        /// </summary>
        /// <returns></returns>
        int GetShiftRequestSequence();

        void UpdateStshiftItemGoldenTaxNo(string GoldenTaxNo, int stSysNo);

        #endregion  移仓单主信息维护

        #region 移仓商品维护

        /// <summary>
        /// 根据移仓单SysNo获取移仓商品信息列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<ShiftRequestItemInfo> GetShiftItemListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建移仓单商品
        /// </summary>
        /// <param name="shiftItem"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        ShiftRequestItemInfo CreateShiftItem(ShiftRequestItemInfo shiftItem, int requestSysNo);

        /// <summary>
        /// 更新移仓单商品
        /// </summary>
        /// <param name="shiftItem"></param>        
        /// <returns></returns>
        ShiftRequestItemInfo UpdateShiftItem(ShiftRequestItemInfo shiftItem);

        /// <summary>
        /// 根据移仓单SysNo，删除其下的商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        void DeleteShiftItemByRequestSysNo(int requestSysNo);

        #endregion 移仓商品维护

        #region 仓库移仓配置
        StockShiftConfigInfo CreateStockShiftConfig(StockShiftConfigInfo info);
        /// <summary>
        /// 返回true表示修改成功，否则修改失败
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateStockShiftConfig(StockShiftConfigInfo info);
        StockShiftConfigInfo GetStockShiftConfigBySysNo(int sysNumber);
        /// <summary>
        /// 根据移入移出仓库编号和移仓类型判断是否已经存在相应记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool IsExistStockShiftConfig(StockShiftConfigInfo info);
        #endregion
    }
}
