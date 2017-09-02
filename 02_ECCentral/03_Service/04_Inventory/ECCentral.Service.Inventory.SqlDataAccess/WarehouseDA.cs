using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IWarehouseDA))]
    public class WarehouseDA : IWarehouseDA
    {
        #region 仓库

        /// <summary>
        /// 根据SysNo获取仓库信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        public WarehouseInfo GetWarehouseInfoBySysNo(int warehouseSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetWarehouseBySysNumber");//说明：EC_Central产品化时 调用的 SQL为：Inventory_GetWarehouseInfoBySysNo 现在是中蛋定制化 不存在渠道仓库所以保持同IPP一致
            dc.SetParameterValue("@SysNo", warehouseSysNo);
            return dc.ExecuteEntity<WarehouseInfo>();
        }

        //（暂定）根据地区获取本地最高优先级的仓库编号
        public string GetlocalWHByAreaSysNo(int areaSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_GetlocalWHByAreaSysNo");

            command.SetParameterValue("@AreaSysNo", areaSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            object obj = command.ExecuteScalar();
            if (obj != null)
            {
                return obj.ToString();
            }
            return null;
        }

        public int NewWarehouseSysNo()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_Insert_GetWarehouseSequenceKey");//说明：EC_Central产品化时 调用的 SQL为：Inventory_Insert_NewWarehouseSysNo 现在是中蛋定制化 不存在渠道仓库所以保持同IPP一致
            return dc.ExecuteScalar<int>();
        }

        /// <summary>
        /// 创建仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public WarehouseInfo CreateWarehouse(WarehouseInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateWarehouse");//说明：EC_Central产品化时 调用的 SQL为：Inventory_CreateWarehouse 现在是中蛋定制化 不存在渠道仓库所以保持同IPP一致
            entity.SysNo = NewWarehouseSysNo();
            dc.SetParameterValue(entity);
            dc.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public WarehouseInfo UpdateWarehouse(WarehouseInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateWarehouse");//说明：EC_Central产品化时 调用的 SQL为：Inventory_UpdateWarehouse 现在是中蛋定制化 不存在渠道仓库所以保持同IPP一致
            dc.SetParameterValue(entity);
            dc.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 获取仓库列表 (此方法视为其他业务提供的，仓库维护不用到此方法)
        /// </summary>      
        /// <param name="companyCode">公司编号</param>  
        /// <returns>仓库列表</returns>
        public List<WarehouseInfo> GetWarehouseListByCompanyCode(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetWarehouseList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<WarehouseInfo, List<WarehouseInfo>>(reader);
            }
        }

        #endregion
    }
}
