using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IWarehouseDA
    {
        #region 仓库

        /// <summary>
        /// 根据SysNo获取仓库信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        WarehouseInfo GetWarehouseInfoBySysNo(int warehouseSysNo);

        /// <summary>
        /// 根据地区获取本地最高优先级的仓库编号
        /// </summary>
        /// <param name="areaSysNo">地区编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>仓库编码</returns>
        string GetlocalWHByAreaSysNo(int areaSysNo, string companyCode);

        /// <summary>
        /// 创建仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        WarehouseInfo CreateWarehouse(WarehouseInfo entity);

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        WarehouseInfo UpdateWarehouse(WarehouseInfo entity);

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <returns>仓库列表</returns>
        List<WarehouseInfo> GetWarehouseListByCompanyCode(string companyCode);

        #endregion

    }
}
