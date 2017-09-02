using System;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IWarehouseOwnerDA))]
    public class WarehouseOwnerDA : IWarehouseOwnerDA
    {
        #region 仓库所有者

        /// <summary>
        /// 根据SysNo获取仓库所有者信息
        /// </summary>
        /// <param name="warehouseOwnerSysNo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo GetWarehouseOwnerInfoBySysNo(int warehouseOwnerSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetWarehouseOwnerBySysNo");
            dc.SetParameterValue("@OwnerSysNo", warehouseOwnerSysNo);
            return dc.ExecuteEntity<WarehouseOwnerInfo>();
        }

        /// <summary>
        /// 创建仓库所有者信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo CreateWarehouseOwner(WarehouseOwnerInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_CreateWarehouseOwner");
            
            command.SetParameterValue("@OwnerID", entity.OwnerID);
            command.SetParameterValue("@Status", (int)entity.OwnerStatus);
            command.SetParameterValue("@OwnerType", (int)entity.OwnerType);
            command.SetParameterValue("@OwnerName", entity.OwnerName);
            command.SetParameterValue("@OwnerMemo", entity.OwnerMemo);      
            command.SetParameterValue("@CreateDate", entity.CreateDate);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            return command.ExecuteEntity<WarehouseOwnerInfo>();
        }

        /// <summary>
        /// 更新仓库所有者信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo UpdateWarehouseOwner(WarehouseOwnerInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateWarehouseOwner");

            command.SetParameterValue("@OwnerSysNo", entity.SysNo);
            command.SetParameterValue("@Status", (int)entity.OwnerStatus);
            command.SetParameterValue("@OwnerType", (int)entity.OwnerType);
            command.SetParameterValue("@OwnerName", entity.OwnerName);
            command.SetParameterValue("@OwnerMemo", entity.OwnerMemo);
            command.SetParameterValue("@EditDate", entity.EditDate);
            command.SetParameterValue("@EditUserSysNo", entity.EditUser.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            return command.ExecuteEntity<WarehouseOwnerInfo>();
        }

        /// <summary>
        /// 根据公司编号取得仓库所有者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<WarehouseOwnerInfo> GetWarehouseOwnerInfoByCompanyCode(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetWarehouseOwnerByCompanyCode");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<WarehouseOwnerInfo>();
        }

        #endregion
    }
}
