using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IWarehouseOwnerDA
    {

        #region 仓库所有者
        /// <summary>
        /// 根据公司编号取得仓库所有者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<WarehouseOwnerInfo> GetWarehouseOwnerInfoByCompanyCode(string companyCode);
        /// <summary>
        /// 根据SysNo获取仓库所有者信息
        /// </summary>
        /// <param name="warehouseOwnerSysNo"></param>
        /// <returns></returns>
        WarehouseOwnerInfo GetWarehouseOwnerInfoBySysNo(int warehouseOwnerSysNo);

        /// <summary>
        /// 创建仓库所有者信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        WarehouseOwnerInfo CreateWarehouseOwner(WarehouseOwnerInfo entity);

        /// <summary>
        /// 更新仓库所有者信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        WarehouseOwnerInfo UpdateWarehouseOwner(WarehouseOwnerInfo entity);

        #endregion
    }
}
